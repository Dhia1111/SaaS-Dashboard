using APIs.Responses;
using Business;
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

        public UserAuthController(
            IUserAuthService authService,
            ITenantIdProvider tenantIdProvider)
        {
            _authService = authService;
            _tenantIdProvider = tenantIdProvider;
        }

        // POST: api/user/auth/login
      [HttpPost("login")]
        public async Task<ActionResult<ApiResult<object>>> Login([FromBody] DtoUserLogIn request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(ApiResult<object>
                .Ok(result, "Login successful"));
        }

        /*   // POST: api/user/auth/refresh
         [HttpPost("refresh")]
         public async Task<ActionResult<ApiResult<string>>> Refresh()
         {
             string? refreshToken = Request.Cookies["refreshToken"];

             if (string.IsNullOrWhiteSpace(refreshToken))
                 throw new ArgumentException("Refresh token not found");

             var newToken = await _authService.RefreshTokenAsync(refreshToken);

             return Ok(ApiResult<string>
                 .Ok(newToken, "Token refreshed successfully"));
         }

         // POST: api/user/auth/logout
         [HttpPost("logout")]
         public async Task<ActionResult<ApiResult<bool>>> Logout()
         {
             await _authService.LogoutAsync();

             Response.Cookies.Delete("refreshToken");

             return Ok(ApiResult<bool>
                 .Ok(true, "Logout successful"));
         }
       */

        // POST: api/user/auth/complete-registration
        [HttpPost("complete-registration")]
        public async Task<ActionResult<ApiResult<bool>>> CompleteRegistration(
            [FromBody] DtoLogIn request)
        {
            string? authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Bearer "))
            {
                throw new ArgumentException("Access token missing");
            }

            string accessToken = authHeader["Bearer ".Length..].Trim();

            int tenantId = _tenantIdProvider.TenantId;

            if (tenantId == 0)
                throw new ArgumentException("TenantId not found");

            var result = await _authService.CompleteRegistrationAsync(
                request,
                tenantId,
                accessToken);

            return Ok(ApiResult<bool>
                .Ok(result, "Registration completed successfully"));
        }
    }
}