using Business.Exceptions;
using Connection.models;
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

       

        Task<bool> CompleteRegistrationAsync
        (
            DtoLogIn request,
            int tenantId,
            string accessToken
        );
    }

    public class UserAuthService : IUserAuthService
    {
        private readonly IUserRepo _userRepo;
        private readonly IPersonRepository _personRepo;
        private readonly IJwtService _jwtService;
        private readonly IGenralHashService _hashService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateHandler _templateHandler;
        private readonly ITenantService _tenantService;
        private readonly IPasswordHashService _passwordHashService;

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

        public async Task<bool> CompleteRegistrationAsync
        (
            DtoLogIn request,
            int tenantId,
            string accessToken
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

            return await _userRepo.UpdateAsync(user);
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
    DtoUserLogIn request
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
                _jwtService.GenerateAccessToken
                (
                    tenant.TenantId,
                    user.Id,
                    user.Role
                );

            string refreshToken =
                _jwtService.GenerateRefreshTokenToken(tenant.TenantId);

            
            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
               
            };
        }


    }
}