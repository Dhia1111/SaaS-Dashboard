using System.ComponentModel.DataAnnotations;

namespace APIs.ConfigClasses
{
    public class ClientInfo
    {
        public string Url { get; set; }= null!;

    }

    public class StripeOptions
    {
        public string SecretKey { get; set; } = null!;
        public string WebhookSecret { get; set; } = null!;
    }

}
