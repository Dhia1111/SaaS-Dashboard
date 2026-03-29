using Microsoft.AspNetCore.Identity;
using Business;
namespace APIs.Hashing
{
    public class PasswordHashService:IPasswordHashService
    {
       
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordHashService(IPasswordHasher<object> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                hashedPassword,
                providedPassword
            );

            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    
    }
}
