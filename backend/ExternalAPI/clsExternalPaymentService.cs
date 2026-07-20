using SharedDto_Enum;
using ExternalAPI.PaymentProvidersConfig;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;




namespace ExternalAPI
{
    public interface IPaymentProvider
    {

        public enPaymentProviders ProviderName { get; }
        public Task<string> PayAsync(ExternalAPI.DtoPayment PaymentDto);

        Task<enGeneralState?> GetPaymentStatusAsync(string providerPaymentId);
    }

    public class DtoPayment
    {
        public int Id { get; set; }

        public string? ProviderPaymentId { get; set; } 
        public string Currency {  get; set; }=null!;

        public long Amount {  get; set; }


    }

    public class Stripe:IPaymentProvider 
    {
        private static class StripeWebhookEvents
        {
            public const string PaymentSucceeded =
                "payment_intent.succeeded";

            public const string PaymentFailed =
                "payment_intent.payment_failed";

            public const string CheckoutCompleted =
                "checkout.session.completed";

            public const string SubscriptionCreated =
                "customer.subscription.created";

            public const string SubscriptionUpdated =
                "customer.subscription.updated";

            public const string SubscriptionDeleted =
                "customer.subscription.deleted";

            public const string InvoicePaid =
                "invoice.paid";

            public const string InvoicePaymentFailed =
                "invoice.payment_failed";
        }

        private readonly StripeClient _stripeClient;
        public Stripe(IOptions<StripeInfo>stripeInfo)
        {
            var StripeInfo = stripeInfo.Value;
                
            _stripeClient =new StripeClient(StripeInfo.SecretKey);
           

            
        }
        public enPaymentProviders ProviderName { get { return enPaymentProviders.stripe; } }
        public async Task<string> PayAsync(DtoPayment payment)
        {
          
          
            string returnUrl = $"http://localhost:5173/check-subscription-status?paymentId={payment.Id}&session_id={{CHECKOUT_SESSION_ID}}";  
                                

            var options = new SessionCreateOptions
            { ExpiresAt= DateTime.UtcNow.AddMinutes(28),
                LineItems = new List<SessionLineItemOptions>
    {
        new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = payment.Amount,  
                Currency = payment.Currency,
                 ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Payment", // Required field
                    },
                 
            },
            Quantity=1,


        },
    },
                Mode = "payment",
                UiMode = "elements",
                ReturnUrl = returnUrl,
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
        {
            { "PaymentId", payment.Id.ToString() }
        }
                },
                Metadata = new Dictionary<string, string>
        {
            { "PaymentId", payment.Id.ToString() }
        }
            };

            var service = new SessionService(_stripeClient);
            var session = await service.CreateAsync(options);

            // Return the client secret
            return session.ClientSecret;
        }

        public async Task<enGeneralState?> GetPaymentStatusAsync(string providerPaymentId)
        {
            var service = new SessionService();

            var paymentIntent = await service.GetAsync("1");
            if (paymentIntent == null)    return null; 
            return GetGeneralState(paymentIntent.Status);
        }

        private static enGeneralState GetGeneralState(
      string stripeEventType)
        {
            return stripeEventType switch
            {
                StripeWebhookEvents.PaymentSucceeded
                    => enGeneralState.Success,

                StripeWebhookEvents.InvoicePaid
                    => enGeneralState.Success,

                StripeWebhookEvents.PaymentFailed
                    => enGeneralState.Failed,

                StripeWebhookEvents.InvoicePaymentFailed
                    => enGeneralState.Failed,

                StripeWebhookEvents.CheckoutCompleted
                    => enGeneralState.Pending,

                StripeWebhookEvents.SubscriptionCreated
                    => enGeneralState.Pending,

                StripeWebhookEvents.SubscriptionUpdated
                    => enGeneralState.Active,

                StripeWebhookEvents.SubscriptionDeleted
                    => enGeneralState.Cancelled,

                _ => throw new NotSupportedException(
                    $"Unsupported Stripe event type: {stripeEventType}")
            };
        }
    }
}
