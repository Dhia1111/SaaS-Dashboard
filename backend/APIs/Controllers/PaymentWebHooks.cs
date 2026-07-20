using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using ExternalAPI;
using ExternalAPI.PaymentProvidersConfig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SharedDto_Enum;
using Stripe;
public class DtoWebhookRequest
{
    public string Payload { get; set; } = null;

    public string Signature { get; set; } = null!;
}
[ApiController]
[Route("api/payment/webhooks")]
public class WebhookController : ControllerBase
{
    private readonly StripeInfo _stripeInfo;
    private readonly IPaymentProvider _stripePaymentProvider;
    private readonly IPlatformSubscriptionService _subscriptionService;
    private readonly ILogger<WebhookController> _logger;
    private readonly IPaymentService _paymentService;
     private static class StripeWebhookEvents
    {
        public const string PaymentSucceeded =
            "payment_intent.succeeded";

        public const string PaymentFailed =
            "payment_intent.payment_failed";

        public const string CheckoutCompleted =
            "checkout.session.completed";

        public const string CheckoutExpired =
            "checkout.session.expired";

        public const string SubscriptionCreated =
            "customer.subscription.created";

        public const string SubscriptionUpdated =
            "customer.subscription.updated";

        public const string SubscriptionDeleted =
            "customer.subscription.deleted";

        public const string InvoicePaid =
            "invoice.paid";
        public const string ChargeRefunded =
          "charge.refunded";

        public const string InvoicePaymentFailed =
            "invoice.payment_failed";
    }

    public WebhookController(
        IOptions<StripeInfo> options,
        IEnumerable<IPaymentProvider> paymentProviders,
        IPaymentService paymentService,
        IPlatformSubscriptionService platformSubscriptionService

        ,ILogger<WebhookController>logger
      )
    {
        _stripeInfo = options.Value;
        _stripePaymentProvider = paymentProviders.Single(e => e.ProviderName == enPaymentProviders.stripe);
        _subscriptionService= platformSubscriptionService;
        _logger = logger;
        _paymentService = paymentService;
        
    }



    [HttpPost("stripe-webhook")]
    public async Task ProcessWebhookAsync(
    )
    {
        var item = await new StreamReader(Request.Body).ReadToEndAsync();
      Request.Headers.TryGetValue("Stripe-Signature",out StringValues value );

        DtoWebhookRequest request = new DtoWebhookRequest() { 
        
            Payload = item.ToString(),
            Signature =value.ToString(),

        };


        var stripeEvent =    EventUtility.ConstructEvent(
                item,
                request.Signature,
                _stripeInfo.WebhookSecret);
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        bool founded = int.TryParse(paymentIntent?.Metadata["PaymentId"], out int res);
        int paymentId = 0;

        switch (stripeEvent.Type)
        {
            case StripeWebhookEvents.PaymentSucceeded:
            case StripeWebhookEvents.CheckoutCompleted:
               
                if (founded)
                {
                    paymentId = res;
                   
                }
                else
                {
                    _logger.LogWarning("Stripe did not provid the Payment Id  Check configration and Payment settings");
                    
                }

                if (await _subscriptionService.ActivateSubscriptionAsync(paymentId))
                {
                    return;
                }
                else
                {
                    _logger.LogWarning("Activating subscription failed where payment Id :{paymentId} ",paymentId);
                }


                    break;

            case StripeWebhookEvents.PaymentFailed:

                DtoPayment? payment=  await _paymentService.GetByIdAsync(paymentId);
                if (payment != null)
                {
                    payment.PaymentStatus =enGeneralState.Failed;

                    if(!await _paymentService.UpdateAsync(payment))
                    {
                        _logger.LogWarning("Failed to Update payment where payment Id :{paymentId} ", paymentId);

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning("Payment with payment Id :{paymentId}  does not exist", paymentId);

                    return;

                }

                break;


            case StripeWebhookEvents.CheckoutExpired:
                DtoPayment? penndingPayment = await _paymentService.GetByIdAsync(paymentId);
                if (penndingPayment != null)
                {
                    penndingPayment.PaymentStatus = enGeneralState.Expired;

                    if (!await _paymentService.UpdateAsync(penndingPayment))
                    {

                        _logger.LogInformation("payment with {Id} set to expired  ",penndingPayment.Id);
                        return;
                    }
                    else
                    {
                        _logger.LogWarning("setting payment with {Id} to expired faild    ", penndingPayment.Id);

                        return;
                    }


                }


                break;

            case StripeWebhookEvents.ChargeRefunded:
               if(! await _subscriptionService.DisActivateSubscriptionWithStatusAsync(paymentId,enGeneralState.eRefunded))
                {
                    _logger.LogWarning("Failed to cancel subscription where payment Id :{paymentId} ", paymentId);
                }
               else
                {
                    _logger.LogInformation("Subscription canceled successfully where payment Id :{paymentId} ", paymentId);
                }
                break;
                        default:

                break;
        }
    }

     
}





public class DtofetchPaymentStatus { 

    public enPaymentProviders Provider { get;set ; }
    public  string ProviderPaymentId { get; set; }=null!;
}


[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase {

    private readonly ILogger<PaymentController> _Logger;
    private readonly IPaymentService _paymentService;
    private readonly IPlatformSubscriptionService _platformSubscriptionService;
    private readonly Dictionary<enPaymentProviders, IPaymentProvider> _paymentProvidersMap;

    public PaymentController(ILogger<PaymentController> logger,
                             IPaymentService paymentService,
                             IPlatformSubscriptionService platformSubscriptionService,
                             IEnumerable<IPaymentProvider> paymentProvider)
    {
        _Logger = logger;
        _paymentService = paymentService;
        _platformSubscriptionService = platformSubscriptionService;
        _paymentProvidersMap = paymentProvider.ToDictionary(p => p.ProviderName, p => p); 
     }

    [HttpPost("payment-status")]
    public async Task<ActionResult<ApiResult<bool>>> GetPaymentStatus([FromBody] DtofetchPaymentStatus request)
    {

        var providerService=_paymentProvidersMap[request.Provider];
        // 1 check the payment in db ? success or faild or not found  return faild status 
         
        DtoPayment? payment=await _paymentService.GetByPaymentProviderIdAsync(request.ProviderPaymentId);

        if (payment == null) {

            throw new ArgumentException("Payment method is not valid ");

        }

        switch (payment.PaymentStatus) {

            case enGeneralState.Success:
                return Ok(ApiResult<bool>.Ok(true));

            case    enGeneralState.Failed:
                return Ok(ApiResult<bool>.Ok(false));

            case enGeneralState.Expired:
                return Ok(ApiResult<bool>.Ok(false));

            case    enGeneralState.Pending:
                {
                    enGeneralState? status = await providerService.GetPaymentStatusAsync(request.ProviderPaymentId);
                    switch ((enGeneralState)status)
                    {
                        case    enGeneralState.Success:
                            await _platformSubscriptionService.ActivateSubscriptionAsync(payment.Id);
                            return Ok(ApiResult<bool>.Ok(true));

                        case enGeneralState.Failed:
                            payment.PaymentStatus =     enGeneralState.Failed;
                            await _paymentService.UpdateAsync(payment);
                            return Ok(ApiResult<bool>.Ok(false));

                        case enGeneralState.Expired:
                            payment.PaymentStatus =   enGeneralState.Expired;
                            await _paymentService.UpdateAsync(payment);
                            return Ok(ApiResult<bool>.Ok(false));

                        default:
                            return Ok(ApiResult<bool>.Ok(false));
                    }
                }

            default:
                return Ok(ApiResult<bool>.Ok(false));
        }


        // 2 you get a pendding payment => make a direct request to payment Provider to Get You the status of the payment

        // 3 if the status == success activate the subscription and complit payment as success

        //   else set payment as faild and return faild payment


    }



}


