using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Net;

public class DtoSignUp
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string CompanyName {  get; set; }
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


namespace Business.EndToEndService
{
    public interface ITentantAuthService
    {

        public Task SignUpAsync(DtoSignUp Request);
        public Task<DtoTokens?> LogInAsync(DtoLogIn request, string Ip);
        public Task<DtoTokens?> RefreshTokens(string Ip, DtoTokens tokens);
        public Task<DtoTokens?> VerifyEmailAsync(string Ip,string Code);
        Task LogOut(string token, string Ip);

    }
    public class TenantAuthService : ITentantAuthService
    {
        private readonly ITenantSessionService _tenantSessionService;
        private readonly IJwtService _jwtService;
        private readonly ITokenHandler _tokenHandler;
        private readonly ITenantService _tenantService;
        private readonly IEmailTemplateHandler _emailTemplateHandler;
        private readonly IEmailSettingsFactory _emailSettingsFactory;
        private readonly ILogger<TenantAuthService> _logger;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IEmailService _EmailService;
        public TenantAuthService(ITenantService tenant, IEmailTemplateHandler emailTemplateHandler,
            ILogger<TenantAuthService> logger,
            IEmailSettingsFactory emailSettingsFactory,
            IPasswordHashService passwordHash,
            ITokenHandler tokenHandler,
            IJwtService jwtService,
            ITenantSessionService tenantSession,
            IEmailService emailService
            )
        {
            _tenantSessionService = tenantSession;
            _jwtService = jwtService;
            _tokenHandler = tokenHandler;
            _tenantService = tenant;
            _emailTemplateHandler = emailTemplateHandler;
            _emailSettingsFactory = emailSettingsFactory;
            _logger = logger;
            _passwordHashService = passwordHash;
            _EmailService = emailService;
        }

        public async Task SignUpAsync(DtoSignUp request)
        {

            var existing = await _tenantService.GetByEmailAsync(request.Email);
            if (existing != null) return;

            var token = Guid.NewGuid().ToString("N");
            var tokenHash = _passwordHashService.Hash(token);

            DtoTenant tenant = new DtoTenant("",request.CompanyName, "", false, DateTime.UtcNow.ToLongDateString(), "", 0, new DtoPerson());
            tenant.CreatedAt = DateTime.UtcNow.ToLongDateString();
            tenant.PasswordHash = _passwordHashService.Hash(request.Password);
            tenant.Person = new DtoPerson
            {
                Email = request.Email,
                IsEmailVeryfied = false,
                EmailVerificationCodeExpiry = DateTime.UtcNow.AddMinutes(12).ToLongDateString(),
                tokenHash = tokenHash,

            };
            var EmailSetting = _emailSettingsFactory.CreateSettings();
            string HtmlTemplate = await _emailTemplateHandler.CreateTemplate(tokenHash);
            var Email = new DtoEmail()
            {
                Subject = "Verify your Email",
                From = EmailSetting.Email,
                To = tenant.Person.Email,
                IsBodyAnHtml = true,
                Body = HtmlTemplate,
            };
            //  regester ever think about the email 
            // to be send by the email background service and not the auth service because the auth service is responsible for the domain logic 
            // of the tenant and not the side effect of sending email
            await _EmailService.AddAsync(Email);
            await _tenantService.AddAsync(tenant);

            // I should not mix side effects with the domine logic because it leads to confusion and 
            // limit the scalability
            /* 
            var EmailSetting = _emailSettingsFactory.CreateSettings();
             string HtmlTemplate = await _emailTemplateHandler.CreateTemplate(tokenHash);
             var Email = new DtoEmail()
             {
                 Subject = "Verify your Email",
                 From = EmailSetting.Email,
                 To = tenant.Person.Email,
                 IsBodyAnHtml = true,
                 Body = HtmlTemplate,
             };
             try
             {
                 await _emailExternalService.SendEmail(Email);
             }
             catch
             {
                 _logger.LogWarning("Verification email failed, will retry later");
                 throw;
             }
             */






        }

        public async Task<DtoTokens?> LogInAsync(DtoLogIn request, string Ip)
        {

            DtoTenant? tenant = await _tenantService.GetByEmailAsync(request.Email);
            if (tenant == null) return null;
            if (_passwordHashService.Verify(tenant.PasswordHash, request.Password))
            {
                string refreshToken = _tokenHandler.GenerateRefreshToken();
                DtoTenantSession session = new DtoTenantSession()
                {
                    CurrentRefreshTokenHash = _passwordHashService.Hash(refreshToken),
                    ExpiresAt = DateTime.UtcNow.AddDays(30).ToLongDateString(),
                    GraceUntil = DateTime.UtcNow.AddSeconds(40).ToLongDateString(),
                    IpAddress = Ip,
                    TenantAgent = null,
                    PreviousRefreshTokenHash = null,
                    RevokedAt = null,
                };
                session.SessionId = await _tenantSessionService.AddAsync(session);
                var AccesToken = _jwtService.GenerateAccessToken(tenant.Id, session.SessionId, (int)Roles.UnverfidAdmine);
                return new DtoTokens() { AccessToken = AccesToken, RefreshToken = refreshToken };

            }
            return null;


        }
        public async Task<DtoTokens?> RefreshTokens(string ip, DtoTokens tokens)
        {
            var hash = _passwordHashService.Hash(tokens.RefreshToken);

            var session = await _tenantSessionService.GetByToken(hash);

            // 1. Session validation
            if (session == null || session.RevokedAt != null)
                return null;


            if (DateTime.TryParse(session.ExpiresAt, out DateTime d) && d <= DateTime.UtcNow)
                return null;
            // 2. Check token position
            bool isCurrent = session.CurrentRefreshTokenHash == hash;

            bool isPrevious =
                session.PreviousRefreshTokenHash == hash &&
                DateTime.TryParse(session.GraceUntil, out DateTime graceUntil) &&
                graceUntil > DateTime.UtcNow;

            // 3. Reuse detection (CRITICAL)
            if (!isCurrent && !isPrevious)
            {
                session.RevokedAt = DateTime.UtcNow.ToString();
                session.RevokedReason = "RefreshTokenReuseDetected";
                session.RevokedByIp = ip;

                await _tenantSessionService.UpdateAsync(session);

                // 🔥 Signal security breach
                throw new SecurityTokenException("Refresh token reuse detected");
            }

            // 4. ROTATION (atomic ideally)
            var newRefresh = _tokenHandler.GenerateRefreshToken();
            var newHash = _passwordHashService.Hash(newRefresh);

            session.PreviousRefreshTokenHash = session.CurrentRefreshTokenHash;
            session.CurrentRefreshTokenHash = newHash;
            DateTime newGraceUntil = DateTime.UtcNow.AddSeconds(60);

            session.LastRefreshedAt = DateTime.UtcNow.ToString();
            session.LastRefreshedIp = ip;

            bool update = await _tenantSessionService.UpdateIfCurrentAsync(newHash, tokens.RefreshToken, newGraceUntil, session.SessionId);
            if (!update) throw new SecurityTokenException("Race condition detected");



            // 5. Generate access token
            var tenant = await _tenantService.GetByIdAsync(session.TenantId);

            var accessToken = _jwtService.GenerateAccessToken(
                session.TenantId,
                session.SessionId,
                tenant.Role
            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh
            };
        }

        public async Task<DtoTokens?> VerifyEmailAsync(string Code, string Ip)
        {
            var hashCode = _passwordHashService.Hash(Code);
            var tenant = await _tenantService.GetByPersonHashSecureCodeAsync(hashCode);
            if (tenant == null) return null;
            if (DateTime.TryParse(tenant.Person.EmailVerificationCodeExpiry, out DateTime expiry) && expiry < DateTime.UtcNow)
                return null;
            tenant.Person.IsEmailVeryfied = true;
            tenant.Person.EmailVerificationCodeExpiry = DateTime.UtcNow.ToLongDateString();
            await _tenantService.UpdateAsync(tenant);

            //Create session and Tokens

            string refreshToken = _tokenHandler.GenerateRefreshToken();
            DtoTenantSession session = new DtoTenantSession()
            {
                CurrentRefreshTokenHash = _passwordHashService.Hash(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(30).ToLongDateString(),
                GraceUntil = DateTime.UtcNow.AddSeconds(40).ToLongDateString(),
                IpAddress = Ip,
                TenantAgent = null,
                PreviousRefreshTokenHash = null,
                RevokedAt = null,
            };
            session.SessionId = await _tenantSessionService.AddAsync(session);
            var AccesToken = _jwtService.GenerateAccessToken(tenant.Id, session.SessionId, (int)Roles.UnverfidAdmine);
            return new DtoTokens() { AccessToken = AccesToken, RefreshToken = refreshToken };

        }

        public async Task LogOut(string token ,string Ip)
        {
            DtoTenantSession? sesion= await _tenantSessionService.GetByToken(_passwordHashService.Hash(token));
            if (sesion != null) return;
            sesion.ExpiresAt = DateTime.UtcNow.ToLongDateString();
            sesion.RevokedAt=DateTime.UtcNow.ToLongDateString() ;
            sesion.RevokedAt = DateTime.UtcNow.ToLongDateString() ;
            sesion.RevokedByIp = Ip;

        }
    }

    }

