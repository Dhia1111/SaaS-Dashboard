using Business;
using Business.Config;
using Connection.models;
using Microsoft.Extensions.Options;
using Quartz;
using SharedDto_Enum;

namespace APIs.BackGroundJobs
{
    public class ManageClientSubscription:IJob
    {
        private readonly ILogger<ManageClientSubscription> _logger;
        private readonly IClientSubscriptionService _clientSubscriptionService;
        private readonly IPlatformSubscriptionService _platformSubscriptionService;
        private readonly ITenantPlanServices _tenantPlanService;
        private readonly ITenantPricingOptionServices _tenantPricingOptionService;
        private readonly ITenantService _tenantService;
        private readonly PlatformInfo _platformInfo;


        public ManageClientSubscription(
            ILogger<ManageClientSubscription> logger,
            IClientSubscriptionService clientSubscriptionService,
            IPlatformSubscriptionService platformSubscriptionService,
            ITenantPlanServices tenantPlanServices,
            ITenantPricingOptionServices tenantPricingOptionService,
            IOptions<PlatformInfo>platformInfo,
            ITenantService tenantService
            )
        {
            _logger = logger;
            _clientSubscriptionService = clientSubscriptionService;
            _platformSubscriptionService = platformSubscriptionService;
            _tenantPlanService = tenantPlanServices;
            _tenantPricingOptionService = tenantPricingOptionService;
            _platformInfo = platformInfo.Value;
            _tenantService = tenantService;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ManageClientSubscription job started at: {Time}", DateTimeOffset.UtcNow);
            try
            {
                // Call the method to manage client subscriptions
                await ManageSubscriptionsAsync();
                _logger.LogInformation("ManageClientSubscription job completed successfully at: {Time}", DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ManageClientSubscription job failed at: {Time}", DateTimeOffset.UtcNow);
                throw; // Re-throw to let Quartz handle the failure (retry, misfire, etc.)
            }
        }

        private async Task ManageSubscriptionsAsync()
        {
            DtoTenant? Tenant = await _tenantService.GetByNameAsync(_platformInfo.TenantName); 
            List<DtoPlatformSubscription>? list=(await _platformSubscriptionService.GetAllWhereIsNotRegisterdAsync()).OrderByDescending(e=>e.EndsAt).ToList();
            List<DtoClientSubscription> clientSubscriptions = new List<DtoClientSubscription>(list.Count);
            List<DtoTenantPricingOption> platformPriceOptions = (await _tenantPricingOptionService.GetAllPlatformPlanPricingOptionsAsync()).ToList();
            List<DtoTenantPlan> PlansList = (await _tenantPlanService.GetAllPlatformPlansWithDependenciesAsync()).ToList();
            Dictionary<int, DtoTenantPlan> planMap = PlansList.ToDictionary(e => e.Id, e => e);
            Dictionary<int, DtoTenantPlan> priceOptionMap = platformPriceOptions.ToDictionary(e => e.Id, e => planMap[e.TenantPlanId]);
           

            for (int i= 0;i < list.Count;i++)
            {
                var platformSubscription = list[i];
                var prevPlatformSubscription = i == 0 ? null : list[i - 1];
                DtoClientSubscription clientSubscription = new DtoClientSubscription
                {
                    CreatedAt = platformSubscription.StartedAt,
                    TenantClientIdentifier = platformSubscription.TenantId.ToString(),
                    IsFree = platformSubscription.IsItFree,
                    GradeStatus = i == 0 ? GetState(priceOptionMap[platformSubscription.PricingOptionId], null, platformSubscription, null) :
                                           GetState(priceOptionMap[platformSubscription.PricingOptionId], priceOptionMap[prevPlatformSubscription.PricingOptionId], platformSubscription, prevPlatformSubscription),


                    TenantId = Tenant.TenantId


                };
                clientSubscriptions.Add(clientSubscription);
                list[i].IsRegisterdToClientSubscription = true;



            }


           bool res= await _clientSubscriptionService.AddRangeAsync(clientSubscriptions);

            if (res)
            {
                await _platformSubscriptionService.UpdateRangeAsync(list);
            }





        }

        private enSubscriptionGradeStatus GetState(DtoTenantPlan curplan ,DtoTenantPlan? prevPlan,DtoPlatformSubscription curentSubscription,DtoPlatformSubscription? prevSubscription)
        {



            if (prevSubscription != null)
            {
                if (curentSubscription.IsItFree) return enSubscriptionGradeStatus.FirstTry;
                if (prevSubscription.IsItFree && !curentSubscription.IsItFree)
                {
                    return enSubscriptionGradeStatus.MoveToPaid;
                }

                // i need prev subscription

                if (curplan.GradeLevel > prevPlan.GradeLevel)
                {
                    return enSubscriptionGradeStatus.Upgrade;
                }
                else if (curplan.GradeLevel == prevPlan.GradeLevel)
                {

                    return enSubscriptionGradeStatus.Renewal;

                }
                else if (curplan.GradeLevel < prevPlan.GradeLevel)
                {

                    return enSubscriptionGradeStatus.Downgrade;
                }

            }

            return enSubscriptionGradeStatus.FirstTry;









        }


    }
}
