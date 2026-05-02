


using Business;
using System.Security.Cryptography;
using System.Text;


namespace APIs.Hashing
{
  

    public sealed class GenralHashService : IGenralHashService
    {
        public string Sha256(string input)
        {
            using var sha = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);

            return Convert.ToHexString(hash); // uppercase hex
        }

        public string Sha256Base64(string input)
        {
            using var sha = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public string HmacSha256(string input, string secret)
        {
            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] data = Encoding.UTF8.GetBytes(input);

            using var hmac = new HMACSHA256(key);
            byte[] hash = hmac.ComputeHash(data);

            return Convert.ToHexString(hash);
        }
    }
}
