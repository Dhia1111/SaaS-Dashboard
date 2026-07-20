// in APIs.ConfigClasses.DataKeyProvider.cs
using Business;
using Business.Config;
using Connection.models;
using Quartz.Impl.AdoJobStore.Common;

namespace APIs.ConfigClasses
{
    
    public class TenantIdProvider : ITenantIdProvider
    {
        private int _Id;
        private readonly HttpContext? context;
        private readonly INamingCookies _cookiesNames;
         private int GetTenantIDFromToken(string? token,IJwtService jwtService)
        {
            if (!string.IsNullOrWhiteSpace(token)) { 

                // Validate token + extract TenantId claim
                string? tenantId =
                    jwtService?.ValidateToken(token)?
                    .FindFirst("TenantId")?
                    .Value;
                if (!string.IsNullOrEmpty(tenantId))
                {
                    if (int.TryParse(tenantId, out int tenantIdValue))
                                            {
                        return tenantIdValue;
                    }
                    return 0;
                }
                return 0;

            }
            return 0;
        }
        public TenantIdProvider(IHttpContextAccessor httpContextAccessor, IJwtService jwtService, INamingCookies cookiesNames)
        {
            context = httpContextAccessor.HttpContext;
            _cookiesNames = cookiesNames;

            string? RefreshToken = context?.Request.Cookies[_cookiesNames.TenantRefreshToken];
            RefreshToken= string.IsNullOrWhiteSpace(RefreshToken) ? context?.Request.Cookies[_cookiesNames.UserRefreshToken]: RefreshToken;
            RefreshToken = string.IsNullOrWhiteSpace(RefreshToken) ? context?.Request.Cookies[_cookiesNames.PlatformRefreshToken] : RefreshToken;
            string? accessToken=null;
            string? authHeader = context?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                 accessToken = authHeader["Bearer ".Length..].Trim();
            }
            this._Id = GetTenantIDFromToken(accessToken, jwtService);
            if (this._Id == 0)
            {
                this._Id = GetTenantIDFromToken(RefreshToken, jwtService);
            }

        }
        public int TenantId { get { return _Id; } }
    
    }
}