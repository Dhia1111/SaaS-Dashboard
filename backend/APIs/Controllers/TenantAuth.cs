

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

        string? IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

       
        DtoTokens? tokens = await _TenantAuthService.SignUpAsync(request, IpAddress);
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
   
    [HttpPost("LogIn")]
    public async Task<ActionResult> LogIn(DtoLogIn request)
    {
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
        await Task.Delay(1000);
        return Ok();

    }

    [HttpPatch("VerifyEmail")]
    public async Task<ActionResult> VerifyEmail(string Code)
    {

        await Task.Delay(100);
        return Ok();
    }


}