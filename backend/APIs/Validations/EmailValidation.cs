using System.Text.RegularExpressions;

namespace APIs.Validations
{


    public static class EmailValidator
    {
        // Example: user@domain.com
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
                RegexOptions.Compiled);

        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }
    }
}
