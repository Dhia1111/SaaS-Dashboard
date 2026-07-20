

using APIs.Responses;
using Business.EndToEndService;
using Connection.models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using APIs.ConfigClasses;
using Business.Config;



[Route("api/auth/tenant")]
[ApiController]

public class TenantAuth : ControllerBase
{
    private readonly ITenantAuthService _TenantAuthService;
    private readonly ClientInfo _clientInfo;
    private readonly INamingCookies _cookiesNames;

    public TenantAuth(ITenantAuthService tenantAuthService,
        IOptions<ClientInfo> clientInfo,
        INamingCookies cookiesNames
      )
    {

        _TenantAuthService = tenantAuthService;
        _clientInfo = clientInfo.Value;
        _cookiesNames = cookiesNames;
    }

    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUp([FromBody] DtoSignUp request)
    {
        Response.Cookies.Delete(_cookiesNames.TenantRefreshToken);
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
        Response.Cookies.Append(_cookiesNames.TenantRefreshToken, tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "/",
            
        });

        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));


    }

   

    [HttpPost("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        Response.Cookies.Delete(_cookiesNames.TenantRefreshToken);
        string? Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        Request.Cookies.TryGetValue(_cookiesNames.TenantRefreshToken, out string? value);
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
        Response.Cookies.Append(_cookiesNames.TenantRefreshToken, tokens.RefreshToken, new CookieOptions
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
        if(email == null || providerId == null || Name == null)
        {
            throw new ArgumentException("Google authentication failed: Missing claims");
        }
        if(Name.Length < 3)
        {
           throw new ArgumentException("Google authentication failed: Name must be at least 3 characters long");
        }
        var dto = new DtoOAuth
        {
            ProviderId = providerId,
            TenantId = 0,
            AuthProvider = AuthProviders.Google, 
            TenantName = Name ,
        };

        var tokens = await _TenantAuthService.OAuth(email, dto,
            Request.HttpContext.Connection.RemoteIpAddress?.ToString());
        Response.Cookies.Delete("ExternalAuthCookie");
        Response.Cookies.Append(_cookiesNames.TenantRefreshToken, tokens.RefreshToken,
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

    [HttpGet("is-name-used")]
    public async Task<ActionResult> IsNameUsed([FromQuery] string tenantName)
    {
        if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentException("TenantId not found");
        var IsUsed = await _TenantAuthService.IsTenantNmaeUsed(tenantName);
        return Ok(ApiResult<bool>.Ok(IsUsed));
    }

   


}