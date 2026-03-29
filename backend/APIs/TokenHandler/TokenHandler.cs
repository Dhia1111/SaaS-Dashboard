using System.Security.Cryptography;
using Business;
namespace APIs.TokenHandler
{
    public  class TokenHandler:ITokenHandler
    {
        public  string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    }
}
