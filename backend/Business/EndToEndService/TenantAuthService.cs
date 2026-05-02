using Business.Exceptions;
using Connection.models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

public class DtoSignUp
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

}
public class DtoLogIn
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
public class DtoTokens {
    public string? AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }=null!;
}
public class DtoVerifyEmail
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string Code { get; set; }

}
public class DtoOAuth
{
    [Required]
    public string? ProviderId { get; set; }

    [Required]
    public int  TenantId { get; set; }

    [Required]
    public AuthProviders AuthProvider { get; set; }


    
}
namespace Business.EndToEndService
{
    public interface ITentantAuthService
    {

        public Task SignUpAsync(DtoSignUp Request);
        public Task<DtoTokens> LogInAsync(DtoLogIn request, string Ip);
        public Task<DtoTokens> RefreshTokens(string Ip, DtoTokens tokens);
        public Task<DtoTokens> VerifyEmailAsync(DtoVerifyEmail dto, string Ip);
        public Task ReSendCode(DtoLogIn request);
        Task LogOut(string token, string Ip);
        Task<DtoTokens> OAuth(string email, DtoOAuth dto, string ip);
    }
    public class TenantAuthService : ITentantAuthService
    {
        private readonly ITenantSessionService _tenantSessionService;
        private readonly IJwtService _jwtService;
        private readonly ITokenHandler _tokenHandler;
        private readonly ITenantService _tenantService;
        private readonly IEmailTemplateHandler _emailTemplateHandler;
        private readonly EmailSettings _settings;
        private readonly ILogger<TenantAuthService> _logger;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IGenralHashService _genralHashService;
        private readonly IEmailService _EmailService;
        private readonly IPersonService _personService;
         public TenantAuthService(ITenantService tenant, IEmailTemplateHandler emailTemplateHandler,
            ILogger<TenantAuthService> logger,
            IOptions<EmailSettings> options,
            IPasswordHashService passwordHash,
            IGenralHashService genralHashService,
            ITokenHandler tokenHandler,
            IJwtService jwtService,
            ITenantSessionService tenantSession,
            IEmailService emailService,
            IPersonService personService
             )
        {
            _tenantSessionService = tenantSession;
            _jwtService = jwtService;
            _tokenHandler = tokenHandler;
            _tenantService = tenant;
            _emailTemplateHandler = emailTemplateHandler;
            _logger = logger;
            _passwordHashService = passwordHash;
            _genralHashService = genralHashService;
            _EmailService = emailService;
            _personService = personService;
            _settings = options.Value;
         }

        // constructor unchanged...

        public async Task SignUpAsync(DtoSignUp request)
        {
            var existing = await _tenantService.GetByEmailAsync(request.Email);

            if (existing != null)
                throw new ResourceAlreadyExistsException("User", request.Email);

            var token = Guid.NewGuid().ToString("N");
            var tokenHash = _passwordHashService.Hash(token);

            var tenant = new DtoTenant
            {
                Role=(int)Roles.Tenant,
                CreatedAt = DateTime.Now.ToString(),
                PasswordHash = _passwordHashService.Hash(request.Password),
                Person = new DtoPerson
                {
                    Email = request.Email,
                    IsEmailVeryfied = false,
                    EmailVerificationCodeExpiry = DateTime.Now.AddMinutes(12).ToString(),
                    SecureCode = tokenHash,
                }
            };

            var html = await _emailTemplateHandler.CreateTemplate(token);

            await _EmailService.AddAsync(new DtoEmail
            {
                Subject = "Verify your Email",
                From = _settings.Email,
                To = tenant.Person.Email,
                IsBodyAnHtml = true,
                Body = html
            });

            await _tenantService.AddAsync(tenant);
        }

        public async Task<DtoTokens> LogInAsync(DtoLogIn request, string ip)
        {
            var tenant = await _tenantService.GetByEmailAsync(request.Email);

            if (tenant == null)
                throw new AuthenticationFailedException();

            if (!_passwordHashService.Verify(tenant?.PasswordHash, request.Password))
                throw new AuthenticationFailedException();

            if (!tenant.Person.IsEmailVeryfied)
                throw new EmailNotVerifiedException();

            var refreshToken = _jwtService.GenerateAccessToken(
                tenant.TenantId,
                -1,
                (int)Roles.Tenant
            );

            var session = new DtoTenantSession
            {
                CurrentRefreshTokenHash = _genralHashService.Sha256(refreshToken),
                ExpiresAt = DateTime.Now.AddDays(30).ToString(),
                GraceUntil = DateTime.Now.AddSeconds(40).ToString(),
                IpAddress = ip,
                TenantId = tenant.TenantId,
             };

            session.SessionId = await _tenantSessionService.AddAsync(session);

          

            var accessToken = _jwtService.GenerateAccessToken(
                tenant.TenantId,
                session.SessionId,
                tenant.Role
            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<DtoTokens> RefreshTokens(string ip, DtoTokens tokens)
        {

            var hash = _genralHashService.Sha256(tokens.RefreshToken);
            var session = await _tenantSessionService.GetByToken(hash);

         

            if (session == null || session.RevokedAt != null)
                throw new AuthenticationFailedException();
            

            if (DateTime.TryParse(session.ExpiresAt, out var exp) && exp <= DateTime.UtcNow)
                throw new AuthenticationFailedException();

            bool isCurrent = session.CurrentRefreshTokenHash == hash;

            bool isPrevious =
                session.PreviousRefreshTokenHash == hash &&
                DateTime.TryParse(session.GraceUntil, out var grace) &&
                grace > DateTime.UtcNow;

            if (!isCurrent && !isPrevious)
            {
                session.RevokedAt = DateTime.Now.ToString();
                session.RevokedReason = "ReuseDetected";
                session.RevokedByIp = ip;

             bool update=   await _tenantSessionService.UpdateAsync(session);

           if(!update)  throw new AuthenticationFailedException();
            }

            var newRefresh = _jwtService.GenerateRefreshTokenToken(
                session.TenantId,
               (int) Roles.Tenant
            );
            var newHash = _genralHashService.Sha256(newRefresh);

            var updated = await _tenantSessionService.UpdateIfCurrentAsync(
                newHash,
                _genralHashService.Sha256(tokens.RefreshToken),
                DateTime.Now.AddSeconds(60),
                session.SessionId
            );

            if (!updated)
                throw new AuthenticationFailedException();

            var tenant = await _tenantService.GetByIdAsync(session.TenantId);

            var accessToken = _jwtService.GenerateAccessToken(
                session.TenantId,
                session.SessionId,
                (int)Roles.Tenant
            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh
            };
        }

        public async Task<DtoTokens> VerifyEmailAsync(DtoVerifyEmail dto, string ip)
        {
            var tenant = await _tenantService.GetByEmailAsync(dto.Email);

            if (tenant == null)
                throw new AuthenticationFailedException();

            if (!_passwordHashService.Verify(tenant.PasswordHash, dto.Password))
                throw new AuthenticationFailedException();

            if (!_passwordHashService.Verify(tenant.Person.SecureCode, dto.Code))
                throw new AuthenticationFailedException();

            if (DateTime.TryParse(tenant.Person.EmailVerificationCodeExpiry, out var exp) && exp < DateTime.UtcNow)
                throw new AuthenticationFailedException();

            if (!tenant.Person.IsEmailVeryfied)
            {
                tenant.Person.IsEmailVeryfied = true;
                await _tenantService.UpdateAsync(tenant);
            }
            var refreshToken = _jwtService.GenerateRefreshTokenToken(
               tenant.TenantId,
                (int)Roles.Tenant   
           );
            var session = new DtoTenantSession
            {
                CurrentRefreshTokenHash = _genralHashService.Sha256(refreshToken),
                ExpiresAt = DateTime.Now.AddDays(30).ToString(),
                GraceUntil = DateTime.Now.AddSeconds(40).ToString(),
                IpAddress = ip,
                TenantId = tenant.TenantId,
             };

            session.SessionId = await _tenantSessionService.AddAsync(session);

           

            var accessToken = _jwtService.GenerateAccessToken(
                tenant.TenantId,
                session.SessionId,
                tenant.Role
            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task ReSendCode(DtoLogIn request)
        {
            var tenant = await _tenantService.GetByEmailAsync(request.Email);

            if (tenant == null)
                throw new AuthenticationFailedException();

            if (!_passwordHashService.Verify(tenant.PasswordHash, request.Password))
                throw new AuthenticationFailedException();

            var code = Guid.NewGuid().ToString();

            tenant.Person.SecureCode = _passwordHashService.Hash(code);
            tenant.Person.EmailVerificationCodeExpiry = DateTime.Now.AddMinutes(12).ToString();

            await _personService.UpdateAsync(tenant.Person);

            var html = await _emailTemplateHandler.CreateTemplate(code);

            await _EmailService.AddAsync(new DtoEmail
            {
                Subject = "Verify your Email",
                From = _settings.Email,
                To = tenant.Person.Email,
                IsBodyAnHtml = true,
                Body = html
            });
        }
        public async Task LogOut(string token, string ip)
        {
            var session = await _tenantSessionService.GetByToken(_genralHashService.Sha256(token));

            if (session == null)
                throw new AuthenticationFailedException();

            session.ExpiresAt = DateTime.Now.ToString();
            session.RevokedAt = DateTime.Now.ToString();
            session.RevokedByIp = ip;

            await _tenantSessionService.UpdateAsync(session);
        }
        public async Task<DtoTokens> OAuth(string email, DtoOAuth dto, string ip)
        {
            var tenant=await _tenantService.GetByEmailAsync(email);

            if (tenant != null)
            {  
                var Session=await _tenantSessionService.GetByTenantId(tenant.TenantId);
                if (Session != null && Session.IpAddress == ip)
                { //update the session 
                   var AcessToken= _jwtService.GenerateAccessToken(
                        tenant.TenantId,
                        Session.SessionId,
                        tenant.Role
                    );
                    var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                        tenant.TenantId,
                         (int)Roles.Tenant
                    );

                    Session = new DtoTenantSession
                    {
                        SessionId = Session.SessionId,
                        PreviousRefreshTokenHash = Session.CurrentRefreshTokenHash,
                        CurrentRefreshTokenHash = _genralHashService.Sha256(RefreshToken),
                        ExpiresAt = DateTime.Now.AddDays(30).ToString(),
                        GraceUntil = DateTime.Now.AddSeconds(40).ToString(),
                        IpAddress = ip,
                        TenantId = tenant.TenantId,
                        LastRefreshedIp = ip,
                        LastRefreshedAt = DateTime.Now.ToString()
                     
                    };
                    await _tenantSessionService.UpdateAsync(Session);
                    return new DtoTokens
                    {
                        AccessToken = AcessToken,
                        RefreshToken = RefreshToken
                    };

                }
                else
                {


                    var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                        tenant.TenantId,
                         (int)Roles.Tenant
                    );

                    Session = new DtoTenantSession
                    {
                        CurrentRefreshTokenHash = _genralHashService.Sha256(RefreshToken),
                        ExpiresAt = DateTime.Now.AddDays(30).ToString(),
                        GraceUntil = DateTime.Now.AddSeconds(40).ToString(),
                        IpAddress = ip,
                        TenantId = tenant.TenantId,
                        LastRefreshedIp = ip,

                        
                    };

                    Session.SessionId=await _tenantSessionService.AddAsync(Session);

                    var AcessToken = _jwtService.GenerateAccessToken(
                      tenant.TenantId,
                      Session.SessionId,
                      tenant.Role
                  );

                    return new DtoTokens
                    {
                        AccessToken = AcessToken,
                        RefreshToken = RefreshToken
                    };


                }


            }
            else
            {
                var newTenant = new DtoTenant
                {
                    Role = (int)Roles.Tenant,
                    CreatedAt = DateTime.Now.ToString(),
                    Person = new DtoPerson
                    {
                        Email = email,
                        IsEmailVeryfied = true,
                        Provider=(int)dto.AuthProvider,
                        ProviderId=dto.ProviderId,
                    },
                   
                    
                };
                newTenant.TenantId=await _tenantService.AddAsync(newTenant);
                var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                     newTenant.TenantId,
                   (int)Roles.Tenant
                );
                var Session = new DtoTenantSession
                {
                    CurrentRefreshTokenHash = _genralHashService.Sha256(RefreshToken),
                    ExpiresAt = DateTime.Now.AddDays(30).ToString(),
                    GraceUntil = DateTime.Now.AddSeconds(40).ToString(),
                    IpAddress = ip,
                    TenantId = newTenant.TenantId,
                    LastRefreshedIp = ip,
                 };
                Session.SessionId=await _tenantSessionService.AddAsync(Session);
                var AcessToken = _jwtService.GenerateAccessToken(
                  newTenant.TenantId,
                  Session.SessionId,
                  newTenant.Role
              );
                return new DtoTokens
                {
                    AccessToken = AcessToken,
                    RefreshToken = RefreshToken
                };



            }

        }
    }

}

    

