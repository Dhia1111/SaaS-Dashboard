

using APIs.AssetHandler;
 using APIs.Hashing;
using APIs.Responses;
using APIs.Validations;
using Business;
using Business.EndToEndService;
using Connection.models;
using ExternalAPI;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


[Route("api/auth/tenant")]
[ApiController]

public class TenantAuth : ControllerBase
{
    private readonly ITentantAuthService _TenantAuthService;
    
   public TenantAuth(ITentantAuthService tenantAuthService) { 
 
        _TenantAuthService = tenantAuthService;
   }

    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUp([FromBody]DtoSignUp request)
     {
        Response.Cookies.Delete("RefreshToken");
        await _TenantAuthService.SignUpAsync(request); 
     
        return Ok(ApiResult<object>.Ok(null));

    }
   
    [HttpPost("LogIn")]
    public async Task<ActionResult> LogIn(DtoLogIn request)
    {
        Response.Cookies.Delete("RefreshToken");
        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();


        DtoTokens? tokens = await _TenantAuthService.LogInAsync(request,IpAddress);
        if (tokens == null)
        {
            throw new Exception("SignUp Service failded");
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "api/auth/tenant/"
        });

        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));


    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult> RefreshToken(DtoTenant tenant)
    {
        Response.Cookies.Delete("RefreshToken");

        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        bool reslt = Request.Cookies.TryGetValue("", out string? RefreshToken);
        if (reslt == false||RefreshToken==null) {

            throw new Exception("Unvalaid Request");
        
        }

        DtoTokens RequestTokens = new DtoTokens
        {
            AccessToken="",
            RefreshToken=RefreshToken,
        };
        
        DtoTokens? tokens = await _TenantAuthService.RefreshTokens(IpAddress, RequestTokens);
        if (tokens == null)
        {
            throw new Exception("SignUp Service failded");
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "api/auth/tenant/"
        });

        return Ok(ApiResult<object>.Ok(new { AccessToken = tokens.AccessToken }));


    }

    [HttpPost("LogOut")]
    public async Task<ActionResult> LogOut(DtoTenant tenant)
    {
        Response.Cookies.Delete("RefreshToken");
        string? Ip=Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        Request.Cookies.TryGetValue("RefreshToken",out string? value);
        if (value == null||string.IsNullOrEmpty(Ip)) return BadRequest(ApiResult<object>.Fail("404","invalid Request"));
        await _TenantAuthService.LogOut(value,Ip);
       
        return Ok(ApiResult<object>.Ok(null));

    }

    [HttpPatch("VerifyEmail")]
    public async Task<ActionResult> VerifyEmail(string Code)
{
        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        if (IpAddress == null) return BadRequest();
       
        DtoTokens? tokens = await _TenantAuthService.VerifyEmailAsync(Code, IpAddress);
        if (tokens == null)
        {
            throw new Exception("SignUp Service failded");
        }
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
            Path = "api/auth/tenant/"
        });
     
        return Ok(ApiResult<object>.Ok(new {AccessToken=tokens.AccessToken}));
    }



}