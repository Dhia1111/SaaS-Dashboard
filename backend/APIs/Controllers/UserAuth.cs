using APIs.Responses;
using Business;
using Business.Config;
using Business.EndToEndService;
using Connection.models;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("api/user/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _authService;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly IJwtService _jwtService;
        private readonly INamingCookies _cookiesNames;
        public UserAuthController(
            IUserAuthService authService,
            ITenantIdProvider tenantIdProvider,
            IJwtService jwtService,
            INamingCookies namingCookies)
        {
            _authService = authService;
            _tenantIdProvider = tenantIdProvider;
            _jwtService = jwtService;
            _cookiesNames = namingCookies;
        }

        // POST: api/user/auth/login
        [HttpPost("login")]
         public async Task<ActionResult<ApiResult<string>>> Login([FromBody] DtoUserLogIn request)
        {
            string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _authService.LoginAsync(request,IpAddress);
            Response.Cookies.Append(_cookiesNames.UserRefreshToken, result.RefreshToken, options: new CookieOptions { HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Path = "/" });

            return Ok(ApiResult<string>
                .Ok(result.AccessToken, "Login successful"));
        }
    
        // POST: api/user/auth/invitations
        [HttpPost("invitations")]
        public async Task<ActionResult<ApiResult<int>>> SendInvitation(
            [FromBody] DtoSendInvitation dto)
        {
            dto.TenantId = _tenantIdProvider.TenantId;

            if (dto.TenantId == 0)
                throw new ArgumentException("TenantId not found");

            var userId = await _authService.SendInvitationAsync(dto);

            return Ok(ApiResult<int>
                .Ok(userId, "Invitation sent successfully"));
        }

       
        // POST: api/user/auth/complete-registration
        [HttpPost("complete-registration")]
        public async Task<ActionResult<ApiResult<string>>> CompleteRegistration(
            [FromBody] DtoLogIn request)
        {
            string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Bearer "))
            {
                throw new ArgumentException("Access token missing");
            }

            string accessToken = authHeader["Bearer ".Length..].Trim();

            if(_jwtService.ValidateAccessToken() == null)
                throw new ArgumentException("Invalid access token");
            int tenantId = _tenantIdProvider.TenantId;

            if (tenantId == 0)
                throw new ArgumentException("TenantId not found");

            var result = await _authService.CompleteRegistrationAsync(
                request,
                tenantId,
                accessToken, IpAddress);
            Response.Cookies.Append(_cookiesNames.UserRefreshToken, result.RefreshToken, options: new CookieOptions { HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Path = "/" });

            return Ok(ApiResult<string>
                .Ok(result.AccessToken, "Registration completed successfully"));
        }

        // post: api/user/auth/logout
        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            Response.Cookies.Delete(_cookiesNames.UserRefreshToken);
            string? Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            Request.Cookies.TryGetValue(_cookiesNames.UserRefreshToken, out string? value);
            if (value == null || string.IsNullOrEmpty(Ip)) throw new ArgumentException();
            await _authService.LogOutAsync(value, Ip);

            return Ok(ApiResult<object>.Ok(null));

        }



    }
}