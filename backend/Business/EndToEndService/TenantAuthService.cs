using Business.Config;
using Business.Exceptions;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

public class DtoSignUp
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }=null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; }=null!;
    [Required]
    [MinLength(3)]
    public string TenantName { get; set; }=null!;

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

    [Required]
    [MinLength(4)]
    public string TenantName { get; set; }=null!;



}
namespace Business.EndToEndService
{
    public interface ITenantAuthService
    {

        public Task SignUpAsync(DtoSignUp Request);
        public Task<DtoTokens> LogInAsync(DtoLogIn request, string Ip);
         public Task<DtoTokens> VerifyEmailAsync(DtoVerifyEmail dto, string Ip);
        public Task ReSendCode(DtoLogIn request);
        Task LogOut(string token, string Ip);
        Task<bool> IsTenantNmaeUsed(string tenantName);
        Task<DtoTokens> OAuth(string email, DtoOAuth dto, string ip);
    }
    public class TenantAuthService : ITenantAuthService, IRefreshTokenService
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
        private readonly INamingCookies _ProprtiesNaming;
        private readonly IPlatformAdmineService _PlatformOwnerService;
        private readonly IPlatformSubscriptionRepo _platformSubscriptionRepo;
        private readonly ITenantPlanRepository _tenantPlanRepo;
        private readonly ITenantPermissionRepository _tenantPermissionRepository;
        private readonly PlatformInfo _platformInfo;
        
        public TenantAuthService(ITenantService tenant,
            IEmailTemplateHandler emailTemplateHandler,
            ILogger<TenantAuthService> logger,
            IOptions<EmailSettings> options,
            IPasswordHashService passwordHash,
            IGenralHashService genralHashService,
            ITokenHandler tokenHandler,
            IJwtService jwtService,
            ITenantSessionService tenantSession,
            IEmailService emailService,
            IPersonService personService,
            INamingCookies proprtiesNaming
, IPlatformAdmineService platformOwnerService,
            IPlatformSubscriptionRepo platformSubscriptionRepo,
            ITenantPlanRepository tenantPlanRepo,
            ITenantPermissionRepository tenantPermissionRepository,
            IOptions<PlatformInfo>platformInfo

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
            _ProprtiesNaming = proprtiesNaming;
            _PlatformOwnerService = platformOwnerService;
            _platformSubscriptionRepo = platformSubscriptionRepo;
            _tenantPlanRepo = tenantPlanRepo;
            _tenantPermissionRepository = tenantPermissionRepository;
            _platformInfo = platformInfo.Value;
        }

        // constructor unchanged...
        public string CookieName { get { return _ProprtiesNaming.TenantRefreshToken; } }
        public async Task SignUpAsync(DtoSignUp request)
        {
            var existing = await _tenantService.GetByEmailOrNameAsync(request.Email, request.TenantName);

            if (existing != null)
                throw new ResourceAlreadyExistsException("User", request.Email);

            var token = Guid.NewGuid().ToString("N");
            var tokenHash = _passwordHashService.Hash(token);

            var tenant = new DtoTenant
            {
                Role = (int)enPlaformRoles.Tenant,
                CreatedAt = DateTime.Now.ToString(),
                PasswordHash = _passwordHashService.Hash(request.Password),
                Name = request.TenantName,
                HaveUsedTheFreeTry =false,
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

            var Owner = await _PlatformOwnerService.GetByTenantIdAsync(tenant.TenantId);
            var refreshToken = _jwtService.GenerateRefreshTokenToken(
                tenant.TenantId
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
                tenant.Role,
                tenant.IsActive ,
                Owner != null,
               (int) (Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
               await GetTenantAuthorizations(tenant)

                





            );

            return new DtoTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        private async Task<long> GetTenantAuthorizations(DtoTenant tenant)
        {
            DtoTenant? Platform = await _tenantService.GetByNameAsync(_platformInfo.TenantName);

            if (Platform == null)
            {
                _logger.LogError("Could not read platfom info");
                throw new Exception("Could not read platform info");
            }

            PlatformSubscription ActiveSubscription = await _platformSubscriptionRepo.GetActiveByTenantIdAsync(tenant.TenantId);
            if (ActiveSubscription != null)
            {
                TenantPlan plan = await _tenantPlanRepo.GetSingleWithDependenciesIgnoreQuerryAsync(Platform.TenantId, ActiveSubscription.TenantPlanPricingOption.TenantPlanId);
                if (plan == null)
                {
                    _logger.LogError("we could not find plan with and Id {Id} and tenantId {tenantId}",Platform.TenantId,ActiveSubscription.TenantPlanPricingOption.TenantPlanId);

                    throw new Exception(" this subscription is not related to any plan (can't fetch the plan )");

                }
                else
                {
                    IReadOnlyList<TenantPermission> PlatFormPermissions = await _tenantPermissionRepository.GetAllByTenantIdWithFilterIgnoreAsync(Platform.TenantId);
                  if(PlatFormPermissions == null || PlatFormPermissions?.Count == 0)
                    {
                        _logger.LogError("we could not fetch Permission for plan with an id {Id} and tenantId {tenantId}", Platform.TenantId, ActiveSubscription.TenantPlanPricingOption.TenantPlanId);

                        throw new Exception("Could not fetch Permissions for plan ");

                    }
                    else
                    {
                        Dictionary<int, long> PermissionsMap = PlatFormPermissions.ToDictionary(e => e.Id, e => e.BitValue);
                        long tenantAuthorizations = plan.Permissions.Sum(e =>  PermissionsMap[e.PermissionId]);
                        return tenantAuthorizations;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        public async Task<DtoTokens?> RefreshTokensAsync(string? ip, DtoTokens tokens)
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

                  throw new AuthenticationFailedException();
            }

            var newRefresh = _jwtService.GenerateRefreshTokenToken(
                session.TenantId
             );
            var newHash = _genralHashService.Sha256(newRefresh);

            var updated = await _tenantSessionService.UpdateIfCurrentAsync(
                newHash,
                _genralHashService.Sha256(tokens.RefreshToken),
                DateTime.Now.AddSeconds(60),
                session.SessionId
            );

            if (!updated)
                throw new ResourceAlreadyExistsException("Resource allready Exsist", session.SessionId.ToString());

            var tenant = await _tenantService.GetByIdAsync(session.TenantId);
            var Owner = await _PlatformOwnerService.GetByTenantIdAsync(session.TenantId);
            var accessToken = _jwtService.GenerateAccessToken(
                session.TenantId,
                (int)enPlaformRoles.Tenant,
                tenant.IsActive,
                Owner!=null,
              (int)  (Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
              await GetTenantAuthorizations(tenant)


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
               tenant.TenantId
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


            var Owner = await _PlatformOwnerService.GetByTenantIdAsync(tenant.TenantId);

            var accessToken = _jwtService.GenerateAccessToken(
                tenant.TenantId,
                tenant.Role,
                tenant.IsActive,
                Owner!=null
                ,
                 (int)(Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
              await GetTenantAuthorizations(tenant)

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
            var tenant=await _tenantService.GetByNameAsync(dto.TenantName);
            var Owner = tenant!=null? await _PlatformOwnerService.GetByTenantIdAsync(tenant.TenantId):null;
            if (tenant != null)
            {
                var Session =await _tenantSessionService.GetByTenantId(tenant.TenantId);
                if (Session != null && Session.IpAddress == ip)
                { //update the session 
                   var AcessToken= _jwtService.GenerateAccessToken(
                        tenant.TenantId,
                        tenant.Role,
                        tenant.IsActive,
                        Owner!=null,
 (int)(Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
              await GetTenantAuthorizations(tenant)
                    );
                    var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                        tenant.TenantId
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
                    // check if tenant name is unique


                    var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                        tenant.TenantId
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
                      tenant.Role,
                      tenant.IsActive,
                      Owner!=null,
  (int)(Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
              await GetTenantAuthorizations(tenant)
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

                bool Unique = !await _tenantService.IsNameUsed(dto.TenantName);
                if (!Unique)
                    throw new ResourceAlreadyExistsException("Tenant", dto.TenantName);

                var newTenant = new DtoTenant
                {
                    Role = (int)enPlaformRoles.Tenant,
                    Name =dto.TenantName,
                    CreatedAt = DateTime.Now.ToString(),
                    Person = new DtoPerson
                    {
                        Email = email,
                        IsEmailVeryfied = true,
                        Provider=(int)dto.AuthProvider,
                        ProviderId=dto.ProviderId,
                    },
                    IsActive=true,
                    HaveUsedTheFreeTry=false,
                   
                    
                };
                newTenant.TenantId=await _tenantService.AddAsync(newTenant);
                var RefreshToken = _jwtService.GenerateRefreshTokenToken(
                     newTenant.TenantId
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
                  newTenant.Role,
                  newTenant.IsActive,
                  Owner!=null,
  (int)(Owner != null ? enPlaformRoles.Owner : enPlaformRoles.Tenant),
              await GetTenantAuthorizations(newTenant)
              );
                return new DtoTokens
                {
                    AccessToken = AcessToken,
                    RefreshToken = RefreshToken
                };



            }

        }

        public async Task<bool> IsTenantNmaeUsed(string tenantName)
        {
                return await _tenantService.IsNameUsed(tenantName);
        }
        
   
    }


}

    

