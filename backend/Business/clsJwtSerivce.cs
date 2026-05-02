using Connection.models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
    public interface IJwtService
    {
        public string GenerateAccessToken(int tenantId, int SessionId, int Roles);
        public string GenerateRefreshTokenToken(int tenantId);

        public ClaimsPrincipal? ValidateToken(string  token);

    }

    public class JwtService : IJwtService
    {
       private readonly JwtSetting _jwtSetting;

        public JwtService(IOptions<JwtSetting> setting)
        {
            _jwtSetting = setting.Value;
        }

        public string GenerateAccessToken(int TenantId, int sessionId,int roles)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, TenantId.ToString()),
            new Claim("SessionId", sessionId.ToString()),
            new Claim("TenantId", TenantId.ToString()),
            new Claim("Roles", roles.ToString()),
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

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSetting.Key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = _jwtSetting.Issuer,
                    ValidAudience = _jwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero // 🔥 no hidden grace
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

}
