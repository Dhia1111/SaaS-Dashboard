

using APIs.Responses;
using Business.EndToEndService;
using Connection.models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using APIs.ConfigClasses;
using Business;
using System.Text;


[Route("api/auth/tenant")]
[ApiController]

public class TenantAuth : ControllerBase
{
    private readonly ITentantAuthService _TenantAuthService;
    private readonly ClientInfo _clientInfo;

    public TenantAuth(ITentantAuthService tenantAuthService, IOptions<ClientInfo> clientInfo)
    {

        _TenantAuthService = tenantAuthService;
        _clientInfo = clientInfo.Value;
    }

    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUp([FromBody] DtoSignUp request)
    {
        Response.Cookies.Delete("RefreshToken");
        await _TenantAuthService.SignUpAsync(request);

        return Ok(ApiResult<object>.Ok(null));

    }

    [HttpPost("LogIn")]
    public async Task<ActionResult> LogIn([FromBody] DtoLogIn request)
    {
         string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();


        DtoTokens? tokens = await _TenantAuthService.LogInAsync(request, IpAddress);
        if (tokens == null)
        {
            throw new Exception("LogIn Service failded");
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "/"
        });

        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));


    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult> RefreshToken()
    {
 
        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        bool reslt = Request.Cookies.TryGetValue("RefreshToken", out string? RefreshToken);
        if (reslt == false || RefreshToken == null)
        {

             throw new ArgumentException(); 

        }

        DtoTokens RequestTokens = new DtoTokens
        {
            AccessToken = "",
            RefreshToken = RefreshToken,
        };

        DtoTokens? tokens = await _TenantAuthService.RefreshTokens(IpAddress, RequestTokens);
        if (tokens == null)
        {
            throw new Exception("SignUp Service failded");
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "/"
        });

        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));


    }

    [HttpPost("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        Response.Cookies.Delete("RefreshToken");
        string? Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        Request.Cookies.TryGetValue("RefreshToken", out string? value);
        if (value == null || string.IsNullOrEmpty(Ip)) throw new ArgumentException();
        await _TenantAuthService.LogOut(value, Ip);

        return Ok(ApiResult<object>.Ok(null));

    }

    [HttpPatch("VerifyEmail")]
    public async Task<ActionResult> VerifyEmail([FromBody] DtoVerifyEmail dto)
    {
        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        if (IpAddress == null) throw new ArgumentException();

        DtoTokens? tokens = await _TenantAuthService.VerifyEmailAsync(dto, IpAddress);
        if (tokens == null)
        {
            throw new ArgumentException();
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "/"
        });
        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));
    }

     

    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback))
        };

        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery]string? url=null)
    {
        var result = await HttpContext.AuthenticateAsync(
            GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded) throw new UnauthorizedAccessException("Google authentication failed");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var providerId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var Name= result.Principal.FindFirstValue(ClaimTypes.Name);

        var dto = new DtoOAuth
        {
            ProviderId = providerId,
            TenantId = 0,
            AuthProvider = AuthProviders.Google,       
        };

        var tokens = await _TenantAuthService.OAuth(email, dto,
            Request.HttpContext.Connection.RemoteIpAddress?.ToString());
        Response.Cookies.Delete("ExternalAuthCookie");
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/",

            });

        return Redirect(_clientInfo.Url);
    }
}