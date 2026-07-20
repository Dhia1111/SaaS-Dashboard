// Business/clsPlatformSubscriptionService.cs
using Business.Exceptions;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class DtoSubscribe {

    [Required]
    public bool requiresPayment { get; set; }

    [Required]
    public int PlanId { get; set; }

    [Required]

    public int PriceOptionId {  get; set; }

     
    public enPaymentProviders? Provider {  get; set; }
 
    public string? ProviderPaymentId { get; set; }



}
public class DtoSubscriptionResult {

    public bool Success { get; set; }

    public bool RequiresPayment { get; set; }

    public int SubscriptionId { get; set; }

    public int? paymentId { get; set; }
    public string? ProviderpaymentId { get; set; }
    public string ?ClientSecret{get; set; }



}

namespace Business
{
    class DtoValidSubscriptionRequestObject {
    
        public DtoTenantPlan TenantPlan {  get; set; }=null!;
        
        public DtoTenantPricingOption PricingOption {  get; set; }=null!;

        public DtoTenant Tenant { get; set; }=null!;

        public DtoTenantFreePlan? FreePlan { get; set; } = null!;

        public bool requiresPayment { get; set; }

        public enPaymentProviders? Provider {  get; set; }

        public string? ProviderPaymentId { get; set; }
    
    }

    public interface IPlatformSubscriptionService : IGenericService<DtoPlatformSubscription>
    {
        Task<DtoPlatformSubscription?> GetActivePlatformSubscriptionAsyncByTenantId(int id);
        PlatformSubscription GetEntity(DtoPlatformSubscription dto);
        Task<DtoSubscriptionResult> Subscribe(DtoSubscribe request);
        Task<bool> ActivateSubscriptionAsync(int paymentId);
        Task<bool> DisActivateSubscriptionWithStatusAsync(int paymentId, enGeneralState status);

        Task<bool> UpdateAsync(DtoPlatformSubscription platform, DtoPayment payment);
        Task<DtoSubscriptionResult> UpGradeSubscription(DtoSubscribe request);

        Task<List<DtoPlatformSubscription>> GetAllWhereIsNotRegisterdAsync();
    }

    public class clsPlatformSubscriptionService : GenericService<DtoPlatformSubscription, PlatformSubscription>, IPlatformSubscriptionService
    {
        private readonly IPlatformSubscriptionRepo _platformSubscriptionRepo;
        private readonly ILogger<clsPlatformSubscriptionService> _Logger;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly ITenantService _tenantService;
        private readonly ITenantPlanServices _tenantPlanService;
        private readonly ITenantFreePlanService _tenantFreePlanService;
        private readonly ITenantPricingCycleServices _tenantPricingCycleServices;
        private readonly IPaymentService _paymentService;
        public clsPlatformSubscriptionService(IPlatformSubscriptionRepo repo,
            ILogger<clsPlatformSubscriptionService> logger
          , ITenantIdProvider tenantIdProvider, ITenantService tenantService,
            ITenantPlanServices tenantPlanServices,
            ITenantFreePlanService FreeplanService,
            ITenantPricingCycleServices tenantPricingCycleServices
, IPaymentService paymentService)
            : base(repo, logger)
        {
            _platformSubscriptionRepo = repo;
            _Logger = logger;
            _tenantIdProvider = tenantIdProvider;
            _tenantService = tenantService;
            _tenantPlanService = tenantPlanServices;
            _tenantFreePlanService = FreeplanService;
            _tenantPricingCycleServices = tenantPricingCycleServices;
            _paymentService = paymentService;
        }

        protected override DtoPlatformSubscription ToDto(PlatformSubscription entity)
        {
            return new DtoPlatformSubscription
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                StartedAt = entity.StartedAt.ToString("O"),
                EndsAt = entity.EndsAt?.ToString("O") ?? null,
                IsActive = entity.IsActive,
                PricingOptionId = entity.TenantPlanPricingOptionId,
                IsItFree = entity.IsItFree,
                IsRegisterdToClientSubscription=entity.IsRegisterdToClientSubscription
                
                

            };
        }
        protected override PlatformSubscription FromDto(DtoPlatformSubscription dto)
        {

            return new PlatformSubscription
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                StartedAt = DateTime.Parse(dto.StartedAt).ToUniversalTime(),
                EndsAt = DateTime.TryParse(dto.EndsAt, out DateTime endDateTime) ? endDateTime.ToUniversalTime() : null,
                IsActive = dto.IsActive,
                TenantPlanPricingOptionId = dto.PricingOptionId,
                IsItFree = dto.IsItFree,
                IsRegisterdToClientSubscription = dto.IsRegisterdToClientSubscription



            };

        }
        public PlatformSubscription GetEntity(DtoPlatformSubscription dto)
        {
            return FromDto(dto);
        }
        public async Task<DtoPlatformSubscription?> GetActivePlatformSubscriptionAsyncByTenantId(int id)
        {
            var res = await _platformSubscriptionRepo.GetActiveByTenantIdAsync(id);

            return res != null ? ToDto(res) : null;

        }
        public async Task<DtoSubscriptionResult> Subscribe(DtoSubscribe request)
        {
            DtoValidSubscriptionRequestObject validSubscriptionRequest = await ValidateSubscription(request);

            var platformSubscription = await this.GetActivePlatformSubscriptionAsyncByTenantId(validSubscriptionRequest.Tenant.TenantId);

            if (platformSubscription != null)
            {

                throw new ResourceAlreadyExistsException("Tenant Subscription", platformSubscription.Id.ToString());


            }
            else
            {
                // check for pendding payments 
                var pendingpayment = await _paymentService.GetLastPendingPaymentAsync();
                bool hasPenddingPayment = pendingpayment != null;
                if (hasPenddingPayment)
                {
                    throw new ResourceAlreadyExistsException("You have a pending payment with Id ", pendingpayment.Id.ToString());
                }
                else
                {
                    // Create new Subscription 

                    var PaymentCycle = (await _tenantPricingCycleServices.GetPlatformPricingCyclesAsync()).Where(e => e.Id == validSubscriptionRequest.PricingOption.TenantPricingCycleId).FirstOrDefault();

                    if (PaymentCycle == null)
                    {

                        throw new ArgumentException($"Platform does not contain any Pricing Cycle with Id {validSubscriptionRequest.PricingOption.TenantPricingCycleId} ");

                    }

                    //Check if the currency is suported 
                    _paymentService.ValidateCurrencySupport(validSubscriptionRequest.PricingOption);


                    var Subscription = new PlatformSubscription
                    {

                        IsActive = false,
                        StartedAt = DateTime.UtcNow,
                        TenantPlanPricingOptionId = validSubscriptionRequest.PricingOption.Id,
                        TenantId = validSubscriptionRequest.Tenant.TenantId,
                        EndsAt = CalcEndPeriodDate(PaymentCycle)

                    };

                    if (!validSubscriptionRequest.requiresPayment)
                    {
                        Subscription.IsActive = true;
                        Subscription.IsItFree = true;
                        int subId = await _platformSubscriptionRepo.AddAsync(Subscription);

                        if (subId == 0) { throw new Exception("Could not add new Subscription "); }

                        return new DtoSubscriptionResult
                        {

                            SubscriptionId = subId,
                            Success = true,
                            RequiresPayment = false,


                        };



                    }

                    else
                    {

                        // Create Transaction With Subscription and payment 
                        Subscription.IsItFree = false;
                        Payment payment = new Payment
                        {
                            PaymentStatus = (int)enGeneralState.Pending,
                            Amount = validSubscriptionRequest.PricingOption.Amount,
                            CreatedAt = DateTime.UtcNow,
                            CompletedAt = null,
                            Currency = validSubscriptionRequest.PricingOption.Currency,
                            Provider = (int)validSubscriptionRequest.Provider,
                            ProviderPaymentId = validSubscriptionRequest.ProviderPaymentId,
                            TenantId = validSubscriptionRequest.Tenant.TenantId,


                        };
                        var subId = await _platformSubscriptionRepo.AddAsync(payment, Subscription);



                        return new DtoSubscriptionResult
                        {

                            SubscriptionId = subId,
                            Success = true,
                            RequiresPayment = true,
                            paymentId = payment.Id,
                            ProviderpaymentId = validSubscriptionRequest.ProviderPaymentId,


                        };

                    }




                }




            }

        }
        private DateTime CalcEndPeriodDate(DtoTenantPricingCycle cycle)
        {

            enTenantPricingCycle pc = (enTenantPricingCycle)Enum.Parse(typeof(enTenantPricingCycle), cycle.PeriodUnit.ToLower(), true);

            switch (pc)
            {

                case enTenantPricingCycle.days:
                    return DateTime.UtcNow.AddDays(cycle.Period);

                case enTenantPricingCycle.weeks:
                    return DateTime.UtcNow.AddDays(cycle.Period * 7);

                case enTenantPricingCycle.months:
                    return DateTime.UtcNow.AddMonths(cycle.Period);

                case enTenantPricingCycle.years:
                    return DateTime.UtcNow.AddYears(cycle.Period);

                case enTenantPricingCycle.hours:
                    return DateTime.UtcNow.AddHours(cycle.Period);
                default:
                    throw new ArgumentException($"Invalid PeriodUnit {cycle.PeriodUnit} for Pricing Cycle {cycle.CycleName} ");


            }




        }
        private async Task<DtoValidSubscriptionRequestObject> ValidateSubscription(DtoSubscribe request)
        {



            DtoTenant tenat = await _tenantService.GetByIdAsync(_tenantIdProvider.TenantId);
            var PlatformPlan = await _tenantPlanService.GetSinglePlatformPlanWithDependenciesAsync(request.PlanId);
            if (PlatformPlan == null || PlatformPlan.PlanPricingOptions == null)
            {

                throw new ArgumentException("platform Does not suport the provided plan ");

            }

            DtoTenantPricingOption? PriceOption = PlatformPlan.PlanPricingOptions.SingleOrDefault(option => option.Id == request.PriceOptionId);

            if (PriceOption == null) throw new ArgumentException("platform Does not support the provided price Option ");

            DtoTenantFreePlan? Freeplan = PlatformPlan.TenantFreePlan;

            // request does not requer payment but the plan does not support free try

            if (Freeplan == null && !request.requiresPayment)
            {
                throw new ArgumentException("platform Does not support a free try for the provided plan ");


            }
            // plan suport free try the tenat have used his free try

            else if (Freeplan != null && tenat.HaveUsedTheFreeTry && !request.requiresPayment)
            {
                throw new ArgumentException("Tenant  HaveUsed The Free Try ");

            }

            return new DtoValidSubscriptionRequestObject
            {

                TenantPlan = PlatformPlan,
                PricingOption = PriceOption,
                Tenant = tenat,
                FreePlan = Freeplan,
                Provider = request.Provider,
                ProviderPaymentId = request.ProviderPaymentId,
                requiresPayment = request.requiresPayment


            };




        }
        public async Task<bool> ActivateSubscriptionAsync(int paymentId)
        {
            return await _platformSubscriptionRepo.ActivateSubscriptionWithQueryFiltersIgnoreAsync(paymentId);
        }
        public async Task<bool> DisActivateSubscriptionWithStatusAsync(int paymentId, enGeneralState status)
        {
            if (status != enGeneralState.Cancelled && status != enGeneralState.Failed && status != enGeneralState.Expired)
            {
                throw new ArgumentException("Invalid status for disactivating subscription. Only Cancelled, Failed, or Expired are allowed.");
            }
            return await _platformSubscriptionRepo.DisActivateSubscriptionWithQueryFiltersIgnoreAsync(paymentId, status);
        }
        public async Task<bool> UpdateAsync(DtoPlatformSubscription platform, DtoPayment payment)
        {


            var entity = FromDto(platform);
            var paymentEntity = _paymentService.GetEntity(payment);
            return await _platformSubscriptionRepo.UpdateAsync(paymentEntity, entity);


        }
        public async Task<DtoSubscriptionResult> UpGradeSubscription(DtoSubscribe request)
        {
            DtoValidSubscriptionRequestObject validSubscriptionRequest = await ValidateSubscription(request);

                    

            // Create Transaction With Subscription and payment 

            var PaymentCycle = (await _tenantPricingCycleServices.GetPlatformPricingCyclesAsync()).Where(e => e.Id == validSubscriptionRequest.PricingOption.TenantPricingCycleId).FirstOrDefault();

            if (PaymentCycle == null)
            {

                throw new ArgumentException($"Platform does not contain any Pricing Cycle with Id {validSubscriptionRequest.PricingOption.TenantPricingCycleId} ");

            }
            var activeSubscription = await _platformSubscriptionRepo.GetActiveByTenantIdAsync(validSubscriptionRequest.Tenant.TenantId);
            //Check if the currency is suported 
          

            if(activeSubscription==null)
            {
                throw new ArgumentException($"Tenant does not have any active subscription to upgrade ");
            }

            TimeSpan? defrence = activeSubscription.EndsAt - DateTime.UtcNow;


            _paymentService.ValidateCurrencySupport(validSubscriptionRequest.PricingOption);
          
            
            var Subscription = new PlatformSubscription
            {

                IsActive = false,
                StartedAt = DateTime.UtcNow,
                TenantPlanPricingOptionId = validSubscriptionRequest.PricingOption.Id,
                TenantId = validSubscriptionRequest.Tenant.TenantId,
                EndsAt = defrence!=null&&activeSubscription.EndsAt>DateTime.UtcNow? CalcEndPeriodDate(PaymentCycle).Add(defrence.Value): CalcEndPeriodDate(PaymentCycle),
                IsItFree = false

            };

            Payment payment = new Payment
            {
                PaymentStatus = (int)enGeneralState.Pending,
                Amount = validSubscriptionRequest.PricingOption.Amount,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null,
                Currency = validSubscriptionRequest.PricingOption.Currency,
                Provider = (int)validSubscriptionRequest.Provider,
                ProviderPaymentId = validSubscriptionRequest.ProviderPaymentId,
                TenantId = validSubscriptionRequest.Tenant.TenantId,


            };
         
            var subId = await _platformSubscriptionRepo.AddAsync(payment, Subscription);

            return new DtoSubscriptionResult
            {

                SubscriptionId = subId,
                Success = true,
                RequiresPayment = true,
                paymentId = payment.Id,
                ProviderpaymentId = validSubscriptionRequest.ProviderPaymentId,


            };


            return null;
        }


        public async Task<List<DtoPlatformSubscription>> GetAllWhereIsNotRegisterdAsync()
        {
            return (await _platformSubscriptionRepo.GetAllWhereIsNotRegisterdAsync()).Select(e=>ToDto(e)).ToList();
        }

        public async Task<bool>ConfirmUpGrade(int paymentId)
        {
            return await _platformSubscriptionRepo.ConfirmUpGradeWithQueryFiltersIgnoreAsync(paymentId);
        }


    }
}
