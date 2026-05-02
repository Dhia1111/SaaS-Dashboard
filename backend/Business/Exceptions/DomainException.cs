namespace Business.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Code { get; }

        protected DomainException(string message, string code) : base(message)
        {
            Code = code;
        }
    }

    public class AuthenticationFailedException : DomainException
    {
        public AuthenticationFailedException()
            : base("Invalid email or password.", "AUTH_FAILED") { }
    }

    public class EmailNotVerifiedException : DomainException
    {
        public EmailNotVerifiedException()
            : base("Email is not verified.", "EMAIL_NOT_VERIFIED") { }
    }

    public class InvalidVerificationCodeException : DomainException
    {
        public InvalidVerificationCodeException()
            : base("Invalid or expired verification code.", "INVALID_CODE") { }
    }

    public class ResourceAlreadyExistsException : DomainException
    {
        public ResourceAlreadyExistsException(string resource, string key)
            : base($"{resource} with '{key}' already exists.", "RESOURCE_EXISTS") { }
    }

    public class SessionExpiredException : DomainException
    {
        public SessionExpiredException()
            : base("Session expired or invalid.", "SESSION_EXPIRED") { }
    }

    public class SecurityBreachException : DomainException
    {
        public SecurityBreachException(string message)
            : base(message, "SECURITY_BREACH") { }
    }
}