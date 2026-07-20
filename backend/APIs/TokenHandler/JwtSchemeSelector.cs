using System.IdentityModel.Tokens.Jwt;

namespace APIs.TokenHandler
{
    public interface IJwtSchemeSelector
    {
        string? Select(HttpContext context);
    }
    public class JwtSchemeSelector : IJwtSchemeSelector
    {
        public string? Select(HttpContext context)
        {
            var auth = context.Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(auth))
                return "D";

            var token = auth["Bearer ".Length..];

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return "D";

            var jwt = handler.ReadJwtToken(token);

            var type = jwt.Claims
                .FirstOrDefault(c => c.Type == "IdentityType")
                ?.Value;

          

            switch (type) {

                case "User": return "UserJwt";;

                case "Tenant": return "TenantJwt";

                default: return"TenantJwt";


            }

        }
   
    
    }
}
