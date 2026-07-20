

using System.ComponentModel.DataAnnotations;

namespace ExternalAPI.PaymentProvidersConfig
{
    public class StripeInfo
    {
        public string SecretKey { get; set; } = null!;
        public string WebhookSecret { get; set; } = null!;


    }
}
