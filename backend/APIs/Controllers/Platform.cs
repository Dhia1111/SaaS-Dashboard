using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Business.Config;
using Connection.models;
using ExternalAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System.Net.WebSockets;
using System.Security.Authentication;


namespace APIs.Controllers
{
    [Route("api/platform/subscription")]
    [ApiController]
    public class PlatformSubscription : ControllerBase
    {
        private readonly ITenantPlanServices _tenantPlanService;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly ITenantService _tenantService;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantPricingCycleServices _PricingCycleService;
        private readonly IDictionary<enPaymentProviders, IPaymentProvider> _extermalPaymentServices;
        private readonly IPlatformSubscriptionService _platformSubscriptionService;
        private readonly IDiscoveryPlatformService _marketService;
        private readonly IPaymentService _paymerService;

        public PlatformSubscription(ITenantPlanServices planServices,
                        ITenantIdProvider tenantIdProvider,
                        IOptions<PlatformInfo> platformInfo,
                        ITenantPricingCycleServices tenantPricingCycle,
                                     IEnumerable<IPaymentProvider> ExternalPaymentSerivces,
                                     IPlatformSubscriptionService platformSubscriptionService,
                                     ITenantService tenantService,
                                     IDiscoveryPlatformService marketService,
                                     IPaymentService paymentService
)
        {
            _platformInfo = platformInfo.Value;
            _tenantPlanService = planServices;
            _tenantIdProvider = tenantIdProvider;
            _PricingCycleService = tenantPricingCycle;
            _extermalPaymentServices = ExternalPaymentSerivces.ToDictionary(x => x.ProviderName, x => x);
            _platformSubscriptionService=platformSubscriptionService;
            _tenantService = tenantService;
            _marketService= marketService;
            _paymerService = paymentService;

        }



        [HttpGet("subscriptions-options")]
            public async Task<ActionResult<IEnumerable<DtoTenantPlan>>> GetSubscriptionListAsync()
        {
            string PlatformTenantName = _platformInfo.TenantName;
            var res = await _tenantPlanService.GetAllWithDependenciesIgnoreQuerryAsync(PlatformTenantName);

            return Ok(ApiResult<IEnumerable<DtoTenantPlan>>.Ok(res));
        }


        [HttpGet("pricng-cycles")]
        public async Task<ActionResult<IEnumerable<DtoTenantPricingCycle>>> PricingCycles()
        {
            string PlatformTenantName = _platformInfo.TenantName;
            var res = await _PricingCycleService.GetAllAsyncWithIgnoreQueryFilter(PlatformTenantName);

            return Ok(ApiResult<IEnumerable<DtoTenantPricingCycle>>.Ok(res));
        }
      
        [Authorize]
        [RequiersdClaim("WriteForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPost("subscribe")]
        public async Task<ActionResult<ApiResult<DtoSubscriptionResult>>> Subscribe([FromBody] DtoSubscribe request)
        {

            if (request.requiresPayment)
            {
                var service = _extermalPaymentServices[(enPaymentProviders)request.Provider];

                DtoSubscriptionResult SubscriptionResult = await _platformSubscriptionService.Subscribe(request);

                if (SubscriptionResult.paymentId == null)
                    throw new Exception("Service Error , platformSubscriptionService faild to create a valid subscription ");
                
                var paymentInfo = await _paymerService.GetByIdAsync(SubscriptionResult.paymentId.Value);

                ExternalAPI.DtoPayment ExternalPayment = new ExternalAPI.DtoPayment
                {
                    Id = SubscriptionResult.paymentId.Value,
                    ProviderPaymentId =SubscriptionResult.ProviderpaymentId ,
                    Currency=paymentInfo.Currency,
                    Amount=(long)(paymentInfo.Amount*100),
                    
                };

                string ClientSecret = await service.PayAsync(ExternalPayment);

                return Ok(ApiResult<DtoSubscriptionResult>.Ok(
                    new DtoSubscriptionResult{
                        ClientSecret= ClientSecret,
                        SubscriptionId=SubscriptionResult.SubscriptionId,
                        Success=SubscriptionResult.Success,
                        paymentId=SubscriptionResult.paymentId,
                        ProviderpaymentId=SubscriptionResult.ProviderpaymentId,
                        RequiresPayment=SubscriptionResult.RequiresPayment,

                    
                    }
                    
                    ));

            }
        
            else
            {
                DtoSubscriptionResult subscriptionResult = await _platformSubscriptionService.Subscribe(request);

                return Ok(ApiResult<DtoSubscriptionResult>.Ok(
                    new DtoSubscriptionResult {
                        SubscriptionId=subscriptionResult.SubscriptionId,
                        RequiresPayment=subscriptionResult.RequiresPayment
                        ,Success=subscriptionResult.Success }));

            }
        }

        [Authorize]
        [RequiersdClaim("WriteForSubscription", SharedDto_Enum.enPlaformRoles.User)]
        [HttpPost("upgrade-subscription")]
        public async Task<ActionResult<ApiResult<DtoSubscriptionResult>>> UpgradeSubscription([FromBody] DtoSubscribe request)
        {

            if (!request.requiresPayment)
            {
                throw new ArgumentException("Upgrade subscription requires payment");
            }
            else
            {
                var service = _extermalPaymentServices[(enPaymentProviders)request.Provider];
                DtoSubscriptionResult SubscriptionResult = await _platformSubscriptionService.UpGradeSubscription(request);
                if (SubscriptionResult.paymentId == null)
                    throw new Exception("Service Error , platformSubscriptionService faild to create a valid subscription ");
                var paymentInfo = await _paymerService.GetByIdAsync(SubscriptionResult.paymentId.Value);
             
                ExternalAPI.DtoPayment ExternalPayment = new ExternalAPI.DtoPayment
                {
                    Id = SubscriptionResult.paymentId.Value,
                    ProviderPaymentId = SubscriptionResult.ProviderpaymentId,
                    Currency = paymentInfo.Currency,
                    Amount = (long)(paymentInfo.Amount * 100),
                };
                string ClientSecret = await service.PayAsync(ExternalPayment);
                return Ok(ApiResult<DtoSubscriptionResult>.Ok(
                    new DtoSubscriptionResult
                    {
                        ClientSecret = ClientSecret,
                        SubscriptionId = SubscriptionResult.SubscriptionId,
                        Success = SubscriptionResult.Success,
                        paymentId = SubscriptionResult.paymentId,
                        ProviderpaymentId = SubscriptionResult.ProviderpaymentId,
                        RequiresPayment = SubscriptionResult.RequiresPayment,
                    }));


            } 
        }

        [Authorize]
        [RequiersdClaim("ReadForSubscription", SharedDto_Enum.enPlaformRoles.User)]

        [HttpGet("tenantUsedFreeTry")]
        public async Task<ActionResult<ApiResult<bool>>> GetIfTenantUsedHisFreeTry([FromQuery] int TenantId)
        {
            int tenantId = _tenantIdProvider.TenantId;
            if (tenantId != TenantId) throw new ArgumentException("TenantId does not match any Tenant ");
            var tenant  =await _tenantService.GetByIdAsync(tenantId);
            
            return Ok(ApiResult<bool>.Ok(tenant.HaveUsedTheFreeTry));
    }

        [Authorize]
        [RequiersdClaim("ReadForSubscription", SharedDto_Enum.enPlaformRoles.User)]

        [HttpGet("active-subscription")]
        public async Task<ActionResult<ApiResult<DtoPlatformSubscription?>>> GetActiveSubscription([FromQuery] int TenantId)
        {
            int tenantId = _tenantIdProvider.TenantId;
            if (tenantId != TenantId) throw new ArgumentException("TenantId does not match any Tenant ");

            var activeSubscription = await _platformSubscriptionService.GetActivePlatformSubscriptionAsyncByTenantId(tenantId);

            return Ok(ApiResult<DtoPlatformSubscription?>.Ok(activeSubscription));
        }


        [HttpGet("marketting-platforms")]
        public ActionResult<ApiResult<List<KeyValuePair<int,string>>>> MarkettingPlatforms()
        {
          

            // atatch payment providers 

               var map = Enum.GetValues(typeof(enMarkettingPlatforms))
                 .Cast<enMarkettingPlatforms>()
                 .Select(x => new KeyValuePair<int, string>(
                     (int)x,
                     x.ToString()))
                 .ToList();



            return Ok(ApiResult< List<KeyValuePair<int, string>>>.Ok(map));


        }



        [HttpGet("payment-providers")]
        public ActionResult<ApiResult<List<KeyValuePair<int,string>>>> PaymentProviders()
        {


            // atatch payment providers 

          var  map = Enum.GetValues(typeof(enPaymentProviders))
                 .Cast<enPaymentProviders>()
                 .Select(x => new KeyValuePair<int, string>(
                     (int)x,
                     x.ToString()))
                 .ToList();



            return Ok(ApiResult<List<KeyValuePair<int, string>>>.Ok(map));


        }


        [HttpPost("subscription-descovery")]
        public async Task<ActionResult<ApiResult<bool>>>SubscriptionDescovery(enMarkettingPlatforms MarkettingPlatform)
        {
            var platform = await _tenantService.GetByNameAsync(_platformInfo.TenantName);

            if (_tenantIdProvider.TenantId == 0) {

                throw new AuthenticationException();
            }
            if (platform == null) {

                throw new Exception("mising configuration to compet this request");

            }

            DtoDiscoveryPlatform m = new DtoDiscoveryPlatform
            {
                MarkettingPlatform = MarkettingPlatform,
                TenantClientIdentifier = _tenantIdProvider.TenantId.ToString(),
                TenantId = platform.TenantId,
              



            };
          var r= await _marketService.AddAsync(m);

            return Ok(ApiResult<bool>.Ok(r != 0));

            }


        [HttpGet("subscription-status")]
        public async Task<ActionResult<ApiResult<bool>>>SubscriptionStatus([FromQuery] int PaymentId)
        {

            if (_tenantIdProvider.TenantId == 0)
            {
                throw new AuthenticationException();
            }
            var payment = await _paymerService.GetByIdAsync(PaymentId);
            if (payment == null || payment.TenantId != _tenantIdProvider.TenantId)
            {
                return NotFound(ApiResult<bool>.Fail("PaymentNotFound", "Payment not found or does not belong to the tenant."));
            }



            return Ok(ApiResult<bool>.Ok(payment.PaymentStatus == enGeneralState.Success));



        }

    
    }
     
    [Route("api/platform/payment")]
    [ApiController]
    public class PlatformPayment:ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IDictionary<enPaymentProviders, IPaymentProvider> _extermalPaymentServices;
        public PlatformPayment(
             IPaymentService paymentService,
             IEnumerable<IPaymentProvider> ExternalPaymentSerivces
             )
        {
            _extermalPaymentServices = ExternalPaymentSerivces.ToDictionary(x => x.ProviderName, x => x);
            _paymentService = paymentService;
        }



    }


}



