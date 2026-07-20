using Business;

namespace APIs.ConfigClasses
{
    public class AccessTokenReader : IAccessTokenReader
    {
        
        private string? _AccessToken;

        public AccessTokenReader(IHttpContextAccessor httpContextAccessor)
        {
           var context=httpContextAccessor.HttpContext;
            string? authHeader = context?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _AccessToken = authHeader["Bearer ".Length..].Trim();
            }
        }

        public string? AccessToken { get { return _AccessToken; } }


    }
}
