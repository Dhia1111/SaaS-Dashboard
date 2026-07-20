using Connection.models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedDto_Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business {

    public class JwtSetting
    {
       public string Key { get; set; } =null!;
       public string Issuer { get; set; } =null!;
       public string Audience { get; set; } =null!;

    }
    public interface IAccessTokenReader {
        
        public string? AccessToken { get; }
    
    }

    public interface IJwtService
    {
        public string GenerateAccessToken(int tenantId, int Roles,bool IsActive ,bool IsTheOwner,int PlatformRole,long TenantAuth);
        public string GenerateRefreshTokenToken(int tenantId);

        public ClaimsPrincipal? ValidateAccessToken();
        public string GenerateAccessTokenForUsers(int UserId,int tenantId, int Roles,int UserAuthorization, bool IsActive ,bool IsAnEmployee,int? EmployeeRole, int? EmployeeAutherizations);
        public ClaimsPrincipal? ValidateToken(string token);


    }
    public class JwtService : JwtTokenReaderBase, IJwtService
    {
        private readonly JwtSetting _jwtSetting;


        public JwtService(IOptions<JwtSetting> setting,IAccessTokenReader accessTokenReader):base(setting,accessTokenReader)
        {
            _jwtSetting = setting.Value;
            
        }

        public string GenerateAccessToken(int TenantId,int roles,bool IsActive , bool IsTheOwner, int PlatformRole,long TenantAuth)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, TenantId.ToString()),
            new Claim("TenantId", TenantId.ToString()),
            new Claim("Roles", roles.ToString()),
            new Claim("IsActive", IsActive.ToString()),
            new Claim("IsTheOwner", IsTheOwner.ToString()),
            new Claim("PlatformRole", PlatformRole.ToString()),
            new Claim("IdentityType", "Tenant"),
            new Claim("IsATenant", "true"),
            new Claim("TenantAuthorizations",TenantAuth.ToString()),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10), // 🔥 short-lived
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshTokenToken(int TenantId)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, TenantId.ToString()),
            new Claim("TenantId", TenantId.ToString()),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30), // 🔥 LongLived-lived
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public new ClaimsPrincipal? ValidateAccessToken()
        {

            return base.ValidateToken();
            
        }
        public new ClaimsPrincipal? ValidateToken(string token)
        {

            return base.ValidateToken(token);

        }

        public string GenerateAccessTokenForUsers( int UserId,int TenantId, int roles,int autherizations,
            bool IsActive, bool IsAnEmployee, 
            int? EmployeeRole, int? EmployeeAutherizations)
        {
            var claims = new[]
            {
            new Claim("UserId", UserId.ToString()),
            new Claim("TenantId", TenantId.ToString()),
            new Claim("Roles", roles.ToString()),
            new Claim("IsActive", IsActive.ToString()),
            new Claim("Authorization", autherizations.ToString()),
            new Claim("IsAnEmployee", IsAnEmployee.ToString()),
            new Claim("EmployeeRole", EmployeeRole.ToString()),
            new Claim("IdentityType", "User"),
            new Claim("IsATenant", "false"),



            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
         
    }

}

namespace Business
{
    public sealed class PlatformTokenClaims
    {
        public int TenantId { get; set; }
        public int Roles { get; set; }
        public bool IsActive { get; set; }
        public bool IsTheOwner { get; set; }
        public int? PlatformRole { get; set; }
    }

    public sealed class UserTokenClaims
    {
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public int Roles { get; set; }
        public int Authorization { get; set; }
        public bool IsAnEmployee { get; set; }
        public int? EmployeeRole { get; set; }
        public int? EmployeeAutherizations { get; set; }
    }

    public interface ITenantTokenReader
    {
        PlatformTokenClaims? ReadToken();
    }

    public interface IUserTokenReader
    {
        UserTokenClaims? ReadToken();
    }

public abstract class JwtTokenReaderBase
    {
        protected readonly JwtSetting _jwtSettings;
        private readonly string? _accessTokem;

        protected JwtTokenReaderBase(IOptions<JwtSetting> jwtSettings,IAccessTokenReader accessTokenReader)
        {
            _jwtSettings = jwtSettings.Value;
            _accessTokem=accessTokenReader.AccessToken;
        }

        protected ClaimsPrincipal? ValidateToken()
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                return handler.ValidateToken(
                    _accessTokem,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = _jwtSettings.Issuer,
                        ValidAudience = _jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_jwtSettings.Key))
                    },
                    out _
                );
            }
            catch
            {
                return null;
            }
        }
        protected ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                return handler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = _jwtSettings.Issuer,
                        ValidAudience = _jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_jwtSettings.Key))
                    },
                    out _
                );
            }
            catch
            {
                return null;
            }
        }

        protected string? GetClaim(
            ClaimsPrincipal principal,
            string claimName)
        {
            return principal.FindFirst(claimName)?.Value;
        }
    }
    public sealed class TenantTokenReader
    : JwtTokenReaderBase,ITenantTokenReader
    {
        public TenantTokenReader(
            IOptions<JwtSetting> jwtSettings,IAccessTokenReader accessTokenReader)
            : base(jwtSettings,accessTokenReader)
        {
        }

        public PlatformTokenClaims? ReadToken()
        {
            var principal = ValidateToken();

            if (principal == null)
                return null;

            if (!int.TryParse(GetClaim(principal, "TenantId"), out var tenantId))
                return null;

            if (!int.TryParse(GetClaim(principal, "Roles"), out var roles))
                return null;

            if (!bool.TryParse(GetClaim(principal, "IsActive"), out var isActive))
                return null;

            if (!bool.TryParse(GetClaim(principal, "IsTheOwner"), out var isOwner))
                return null;

            int? platformRole = null;

            if (int.TryParse(GetClaim(principal, "PlatformRole"), out var role))
                platformRole = role;

            return new PlatformTokenClaims
            {
                TenantId = tenantId,
                Roles = roles,
                IsActive = isActive,
                IsTheOwner = isOwner,
                PlatformRole = platformRole
            };
        }
    }
    public sealed class UserTokenReader
        : JwtTokenReaderBase,
          IUserTokenReader
    {
        public UserTokenReader(
            IOptions<JwtSetting> jwtSettings,IAccessTokenReader accessTokenReader)
            : base(jwtSettings,accessTokenReader)
        {
        }

        public UserTokenClaims? ReadToken()
        {
            var principal = ValidateToken();

            if (principal == null)
                return null;

            if (!int.TryParse(GetClaim(principal, "UserId"), out var userId))
                return null;

            if (!int.TryParse(GetClaim(principal, "TenantId"), out var tenantId))
                return null;

            if (!int.TryParse(GetClaim(principal, "Roles"), out var roles))
                return null;

            if (!int.TryParse(GetClaim(principal, "Authorization"), out var authorization))
                return null;

            if (!bool.TryParse(GetClaim(principal, "IsAnEmployee"), out var isEmployee))
                return null;

            int? employeeRole = null;
            int? employeeAuth = null;

            if (int.TryParse(GetClaim(principal, "EmployeeRole"), out var role))
                employeeRole = role;

            if (int.TryParse(GetClaim(principal, "EmployeeAutherizations"), out var auth))
                employeeAuth = auth;

            return new UserTokenClaims
            {
                UserId = userId,
                TenantId = tenantId,
                Roles = roles,
                Authorization = authorization,
                IsAnEmployee = isEmployee,
                EmployeeRole = employeeRole,
                EmployeeAutherizations = employeeAuth
            };
        }
    }

}
