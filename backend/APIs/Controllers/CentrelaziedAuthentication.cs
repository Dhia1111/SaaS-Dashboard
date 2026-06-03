using APIs.Responses;
using Business;
 using Business.EndToEndService;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace APIs.Controllers
{
    [Route("api/auth")]
    [ApiController]

    public class Auth : ControllerBase
    {
        private readonly Dictionary<string, IRefreshTokenService> _refreshMap;
        public Auth(IEnumerable<IRefreshTokenService> refreshServices)
        {
            _refreshMap = refreshServices.ToDictionary(
                x => x.CookieName,
                x => x);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            foreach (var kvp in _refreshMap)
            {
                var cookieName = kvp.Key;
                var service = kvp.Value;

                if (Request.Cookies.TryGetValue(cookieName, out var refreshToken))
                {
                    var result = await service.RefreshTokensAsync(
                        ip,
                        new DtoTokens
                        {
                            RefreshToken = refreshToken,
                            AccessToken = null
                        });

                    Response.Cookies.Append(cookieName, result.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });
                    return Ok(ApiResult<string>.Ok(
                        result.AccessToken,
                        "Token refreshed successfully"));

                }
            }

            throw new UnauthorizedAccessException("No valid refresh token found in cookies");

        }
    }
}
