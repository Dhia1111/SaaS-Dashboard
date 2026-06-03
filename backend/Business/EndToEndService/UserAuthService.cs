using Business.Config;
using Business.EndToEndService;
using Business.Exceptions;
using Connection.models;
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;

public class DtoSendInvitation
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public int TenantId { get; set; } 

    [Required]
    public Roles Role { get; set; }

    [Required]
    public int UserAuthorization { get; set; }
}
public class DtoUserLogIn
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string TenantName { get; set; } = null!;
}
namespace Business
{
    public interface IUserAuthService
    {
        Task<int> SendInvitationAsync
        (
            DtoSendInvitation request
        );




        Task<DtoTokens?> CompleteRegistrationAsync
        (
            DtoLogIn request,
            int tenantId,
            string accessToken,
            string ip
        );
        Task<DtoTokens> LoginAsync(
            DtoUserLogIn request,string ip
        );

        

        Task LogOutAsync(string token, string ip);
    }

    public class UserAuthService : IUserAuthService, IRefreshTokenService
    {
        private readonly IUserRepo _userRepo;
        private readonly IPersonRepository _personRepo;
        private readonly IJwtService _jwtService;
        private readonly IGenralHashService _hashService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateHandler _templateHandler;
        private readonly ITenantService _tenantService;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IUserSessionRepo _userSessionRepo;
        private readonly INamingCookies _namingProprties;

        public UserAuthService
        (
            IUserRepo userRepo,
            IPersonRepository personRepo,
            IJwtService jwtService,
            IGenralHashService hashService,
            IEmailService emailService,
            IEmailTemplateHandler templateHandler,
            ITenantService tenantService,
            IPasswordHashService passwordHashService
            ,IUserSessionRepo userSessionRepo,
            INamingCookies namingProprties

        )
        {
            _userRepo = userRepo;
            _personRepo = personRepo;
            _jwtService = jwtService;
            _hashService = hashService;
            _emailService = emailService;
            _templateHandler = templateHandler;
            _tenantService = tenantService;
            _passwordHashService = passwordHashService;
            _userSessionRepo = userSessionRepo;
            _namingProprties = namingProprties;
        }

        public string CookieName{get{
                return _namingProprties.UserRefreshToken;
            }
        }

        public async Task<int> SendInvitationAsync
        (
            DtoSendInvitation request
        )
        {
            var tenant =
                await _tenantService.GetByIdAsync(request.TenantId);

            if (tenant == null || tenant.Person == null)
                throw new ArgumentException("Tenant not found");

            if (tenant.Person.Email == request.Email)
                throw new ArgumentException
                (
                    "Cannot invite tenant email"
                );

            var existingUser =
                await _userRepo.GetByEmailAsync(request.Email);

            if
            (
                existingUser != null &&
                existingUser.Person?.IsVeryfied == true
            )
            {
                throw new ResourceAlreadyExistsException
                (
                    "User exists",
                    request.Email
                );
            }

            var token =
                _jwtService.GenerateAccessToken
                (
                    request.TenantId,
                    0,
                    (int)request.Role
                );

            var html =
                await _templateHandler
                .CreateTemplateForUsers(token);

            if (existingUser != null)
            {
                existingUser.Person!.SecureCode =
                    _hashService.Sha256(token);
                bool updated =
                    await _personRepo
                    .UpdateAsync(existingUser.Person);

                if (!updated)
                {
                    throw new ScadulingDataException
                    (
                        "Unable to update secure code"
                    );
                }

                await RegisterInvitationEmail
                (
                    tenant.Person.Email,
                    request.Email,
                    html
                );

                return existingUser.Id;
            }

            var user = new User
            {
                Role = (int)request.Role,
                Authorization = request.UserAuthorization,
                TenantId = request.TenantId,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                Person = new Person
                {
                    Email = request.Email,
                    TenantId = request.TenantId,
                    IsVeryfied = false,
                    SecureCode = _hashService.Sha256(token),
                    EmailVerificationCodeExpiry =
                        DateTime.UtcNow.AddMinutes(12)
                }
            };

            int userId = await _userRepo.AddAsync(user);

            if (userId == 0)
            {
                throw new ScadulingDataException
                (
                    "Unable to create user"
                );
            }

            await RegisterInvitationEmail
            (
                tenant.Person.Email,
                request.Email,
                html
            );

            return userId;
        }

        public async Task<DtoTokens?> CompleteRegistrationAsync
        (
            DtoLogIn request,
            int tenantId,
            string accessToken,
            string ip
        )
        {
            var user =
                await _userRepo.GetByEmailAsync(request.Email);

            if
            (
                user == null ||
                user.Person == null ||
                user.TenantId != tenantId
            )
            {
                throw new ArgumentException
                (
                    "User not found"
                );
            }

            if
            (
                user.Person.SecureCode !=
                _hashService.Sha256(accessToken)
            )
            {
                throw new ArgumentException
                (
                    "Invalid token"
                );
            }

            user.PasswordHash =
                _passwordHashService.Hash(request.Password);

            user.UpdatedAt = DateTime.UtcNow;

            user.Person.IsVeryfied = true;
            user.IsActive = true;

            bool result = await _userRepo.UpdateAsync(user);
            if (!result)
            {
                throw new ScadulingDataException("Unable to update user");
            }
            var refreshToken =
                _jwtService.GenerateRefreshTokenToken(tenantId);
            var session = new UserSession
            {
                CurrentRefreshTokenHash = _hashService.Sha256(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                GraceUntil = DateTime.UtcNow.AddSeconds(40),
                IpAddress = ip,
                TenantId =tenantId,
            };

            session.Id = await _userSessionRepo.AddAsync(session);

            var AccessToken =
                _jwtService.GenerateAccessTokenForUsers
                (
                    tenantId,
                    session.Id
                    ,
                    user.Role,
                    user.Authorization
                );
            
            return new DtoTokens
            {
                AccessToken = AccessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task RegisterInvitationEmail
        (
            string from,
            string to,
            string body
        )
        {
            int emailId =
                await _emailService.AddAsync(new DtoEmail
                {
                    Subject = "Verify your Email",
                    From = from,
                    To = to,
                    IsBodyAnHtml = true,
                    Body = body
                });

            if (emailId == 0)
            {
                throw new ScadulingDataException
                (
                    "Unable to schedule email"
                );
            }
        }

        public async Task<DtoTokens> LoginAsync
(
    DtoUserLogIn request,string ip
)
        {
            var tenant =
                await _tenantService
                .GetByNameAsync(request.TenantName);

            if (tenant == null)
                throw new ArgumentException("Invalid credentials");

            var user =
                await _userRepo.GetByEmailWithTenantIdUnSafeAsync
                (
                    request.Email,  tenant.TenantId            );

            if (user == null || user.Person == null)
                throw new ArgumentException("Invalid credentials");

            bool validPassword =
                _passwordHashService.Verify
                (
                    request.Password,
                    user.PasswordHash
                );

            if (!validPassword)
                throw new ArgumentException("Invalid credentials");

            if (!user.Person.IsVeryfied)
                throw new ArgumentException("Email not verified");

            string accessToken =
                _jwtService.GenerateAccessTokenForUsers
                (
                    tenant.TenantId,
                    user.Id,
                    user.Role
                    ,user.Authorization
                );

            string refreshToken =
                _jwtService.GenerateRefreshTokenToken(tenant.TenantId);

            var session = new UserSession
            {
                CurrentRefreshTokenHash = _hashService.Sha256(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                GraceUntil = DateTime.UtcNow.AddSeconds(40),
                IpAddress = ip,
                TenantId = tenant.TenantId,
            };

            session.Id = await _userSessionRepo.AddAsync(session);

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
               
            };
        }

        public async Task<DtoTokens> RefreshTokensAsync( string? ip,DtoTokens tokens )
        {

            var hash = _hashService.Sha256(tokens.RefreshToken);
            var session = await _userSessionRepo.GetByToken(hash);



            if (session == null || session.RevokedAt != null)
                throw new AuthenticationFailedException();


            if (session.ExpiresAt <= DateTime.UtcNow)
                throw new AuthenticationFailedException();

            bool isCurrent = session.CurrentRefreshTokenHash == hash;

            bool isPrevious =
                session.PreviousRefreshTokenHash == hash &&
                session.GraceUntil > DateTime.UtcNow;

            if (!isCurrent && !isPrevious)
            {
                session.RevokedAt = DateTime.UtcNow;
                session.RevokedReason = "ReuseDetected";
                session.RevokedByIp = ip;

                bool update = await _userSessionRepo.UpdateAsync(session);

                if (!update) throw new AuthenticationFailedException();
            }

            var newRefresh = _jwtService.GenerateRefreshTokenToken(
                session.TenantId
             );
            var newHash = _hashService.Sha256(newRefresh);  
            var updated = await _userSessionRepo.UpdateIfCurrentAsync(
                newHash,
                _hashService.Sha256(tokens.RefreshToken),
                DateTime.Now.AddSeconds(60),
                session.Id
            );

            if (!updated)
                throw new AuthenticationFailedException();

            var user = await _userRepo.GetByIdAsync(session.UserId);
            if (user == null) throw new AuthenticationFailedException(); 
            var accessToken = _jwtService.GenerateAccessTokenForUsers(
                session.TenantId,
                session.Id,
                (int)user.Role,
                (int)user.Authorization
                
            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh
            };
        }

        public async Task LogOutAsync(string token, string ip)
        {
            var session = await _userSessionRepo.GetByToken(_hashService.Sha256(token));

            if (session == null)
                throw new AuthenticationFailedException();

            session.ExpiresAt = DateTime.UtcNow;
            session.RevokedAt = DateTime.UtcNow;
            session.RevokedByIp = ip;

            await _userSessionRepo.UpdateAsync(session);
        }



    }
}