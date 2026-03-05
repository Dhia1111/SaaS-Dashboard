using System.Text.RegularExpressions;

namespace APIs.Validations
{

    public static class StringComplexityValidator
    {
        public static bool IsValid(
            string value,
            int minLength,
            int minLetters,
            int minDigits)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (value.Length < minLength)
                return false;

            int letters = 0;
            int digits = 0;

            foreach (var c in value)
            {
                if (char.IsLetter(c)) letters++;
                else if (char.IsDigit(c)) digits++;
            }

            return letters >= minLetters && digits >= minDigits;
        }
    }

}
