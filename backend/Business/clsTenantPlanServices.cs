using Business.Config;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System.Numerics;


namespace Business
{
    public interface ITenantPlanServices: IGenericService<DtoTenantPlan>
    {

        Task<int> AddNewTenantPlanWithDependancies(DtoAddNewTenantPlan plan);
        Task<bool>IsTenantPlanNameExist(string name);
        Task<IEnumerable<DtoTenantPlan>> GetAllWithDependenciesAsync();

        Task<bool> UpdateTenantPlanWithDependancies(DtoAddNewTenantPlan plan);

        public Task<IEnumerable<DtoTenantPlan>> GetAllWithDependenciesIgnoreQuerryAsync(string TenantName);

        public Task<DtoTenantPlan> GetSingleWithDependenciesIgnoreQuerryAsync(string TenantName,string PlanName);

        public Task<IEnumerable<DtoTenantPlan>> GetAllPlatformPlansWithDependenciesAsync();

        public Task<DtoTenantPlan?> GetSinglePlatformPlanWithDependenciesAsync(string PlanName);
        public Task<DtoTenantPlan?> GetSinglePlatformPlanWithDependenciesAsync(int Id);


    }



    public class clsTenantPlanServices : GenericService<DtoTenantPlan, TenantPlan>, ITenantPlanServices
    {


        private readonly ITenantPlanRepository _tenantPlanRepository;
        private readonly ILogger<clsTenantPlanServices> _logger;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly ITenantPlanPermissionServices _tenantPlanPermissionServices;
        private readonly ITenantPlanBenifestServices _tenantPlanBenifestServices;
        private readonly ITenantPricingOptionServices _tenantPricingOptionServices;
        private readonly ITenantRepo _tenantRepo;
        private readonly ITenantFreePlanService _tenantFreePlanService;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantFreePlanService _freePlanService;
        private readonly ITenantPricingCycleServices _tenantPricingCycleService;



        public clsTenantPlanServices(ILogger<clsTenantPlanServices> logger,  ITenantPlanRepository repo,
            ITenantIdProvider tenantIdProvider,
            ITenantPlanPermissionServices tenantPlanPermissionServices,
            ITenantPlanBenifestServices tenantPlanBenifestServices,
            ITenantPricingOptionServices tenantPricingOptionServices,
            ITenantRepo tenantRepo,
            IOptions<PlatformInfo> platformInfo,
            ITenantFreePlanService tenantFreePlanService,
            ITenantPricingCycleServices tenantPricingCycleServices,
            ITenantFreePlanService freePlanService)
               : base(repo, logger)
        {
            _tenantPlanRepository = repo;
            _logger = logger;
            _tenantIdProvider = tenantIdProvider;
            _tenantPlanPermissionServices = tenantPlanPermissionServices;
            _tenantPlanBenifestServices = tenantPlanBenifestServices;
            _tenantPricingOptionServices = tenantPricingOptionServices;
            _tenantRepo = tenantRepo;
            _platformInfo = platformInfo.Value;
            _tenantFreePlanService = tenantFreePlanService;
            _freePlanService = freePlanService;
            _tenantPricingCycleService = tenantPricingCycleServices;
        }

        protected override TenantPlan FromDto(DtoTenantPlan dto)
        {
            return new TenantPlan
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                Description = dto.Description,
                IsActive = dto.IsActive,
                Name = dto.Name,
                GradeLevel=dto.GradeLevel,
                HasFreeTryOption =dto.HasFreeTryOption,
                Permissions = dto.Permissions != null && dto.Permissions.Count() > 0 ? dto.Permissions.Select(perm => _tenantPlanPermissionServices.GetEntity(perm)).ToList() : null,
                Benefits = dto.Benefits != null && dto.Benefits.Count() > 0 ? dto.Benefits.Select(ben => _tenantPlanBenifestServices.GetEntity(ben)).ToList() : null,
                PricingOptions = dto.PlanPricingOptions != null && dto.PlanPricingOptions.Count() > 0 ? dto.PlanPricingOptions.Select(priceOption => _tenantPricingOptionServices.GetEntity(priceOption)).ToList() : null,
                TenantFreePlan = dto.TenantFreePlan != null ? _freePlanService.GetEntity(dto.TenantFreePlan) : null






            }
            ;
        }
        protected override DtoTenantPlan ToDto(TenantPlan entity)
        {
            return new DtoTenantPlan
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                Description = entity.Description,
                IsActive = entity.IsActive,
                GradeLevel= entity.GradeLevel,
                Name = entity.Name,
                HasFreeTryOption = entity.HasFreeTryOption,
                Permissions = entity.Permissions != null && entity.Permissions.Count > 0 ? entity.Permissions.Select(perm => _tenantPlanPermissionServices.GetDto(perm)) : null,
                Benefits = entity.Benefits != null && entity.Benefits.Count > 0 ? entity.Benefits.Select(ben => _tenantPlanBenifestServices.GetDto(ben)) : null,
                PlanPricingOptions = entity.PricingOptions != null && entity.PricingOptions.Count > 0 ? entity.PricingOptions.Select(priceOption => _tenantPricingOptionServices.GetDto(priceOption)) : null,
                TenantFreePlan=entity.TenantFreePlan!=null?_freePlanService.GetDto(entity.TenantFreePlan):null 
                



            }
            ;
        }
        public async Task<int> AddNewTenantPlanWithDependancies(DtoAddNewTenantPlan plan)
        {
            // select all plans 


            if (plan.TenantPlanBenefits.Count() == 0 || plan.TenantPlanPermissions.Count() == 0 || plan.TenantPricingOptions.Count() == 0||(plan.TenantPlan.HasFreeTryOption&&plan.TenantFreePlan==null)) throw new ArgumentException();
            //set TenantId for all the dependancie
            
            plan.TenantPlan.TenantId = _tenantIdProvider.TenantId;
            plan.TenantPlan.GradeLevel = await SetGradeLevelForPlanAsync(FromDto(plan.TenantPlan));
            if (plan.TenantFreePlan != null) plan.TenantFreePlan.TenantId = _tenantIdProvider.TenantId;
            foreach (var benefit in plan.TenantPlanBenefits)
            {
                benefit.TenantId = _tenantIdProvider.TenantId;
            }
            foreach (var pricingOption in plan.TenantPricingOptions)
            {
                pricingOption.TenantId = _tenantIdProvider.TenantId;
            }
            foreach (var permission in plan.TenantPlanPermissions)
            {
                permission.TenantId = _tenantIdProvider.TenantId;
            }


            var freeplan = plan.TenantFreePlan == null ? null : _tenantFreePlanService.GetEntity(plan.TenantFreePlan);
            if(freeplan!=null)    freeplan.StartAt = DateTime.UtcNow;

            return await _tenantPlanRepository.AddNewTenantPlan(FromDto(plan.TenantPlan),
                   plan.TenantPlanPermissions.Select(d => _tenantPlanPermissionServices.GetEntity(d)).ToList(),
                   plan.TenantPlanBenefits.Select(d => _tenantPlanBenifestServices.GetEntity(d)).ToList(),
                   plan.TenantPricingOptions.Select(d => _tenantPricingOptionServices.GetEntity(d)).ToList()
                   ,freeplan );





        }
        private async Task<int>SetGradeLevelForPlanAsync(TenantPlan newTenantPlan)
        {
            var newprice = newTenantPlan.PricingOptions.First();
            int lestExpensivePlanId = 0;
            int CountOfPlans = 0;

            if (newTenantPlan == null || newTenantPlan.PricingOptions == null || newTenantPlan.PricingOptions.Count() == 0)
            {
                throw new ArgumentException("Pricing Options are required to set the grade level for the plan");
            }

            int GradeLevel = 0;
            var allPlans = (await _tenantPlanRepository.GetAllWithDependenciesAsync())
                .OrderByDescending(p => p.GradeLevel)
                .ToList();
            if (allPlans.Count() == 0) return 0;

 
            for(CountOfPlans = 0; CountOfPlans < allPlans.Count; CountOfPlans++)
            {
                var plan = allPlans[CountOfPlans];
               
                lestExpensivePlanId = await GetThenLeastExpensivePlanAsync(newprice, plan.PricingOptions.First());
                if (lestExpensivePlanId == newTenantPlan.Id)
                {
                    plan.GradeLevel = GradeLevel + 1;
                    await _tenantPlanRepository.UpdateAsync(plan);
                    continue;
                    
                }
                else
                {
                    GradeLevel = plan.GradeLevel+1;

                    break;


                }
            }






            return GradeLevel;

        }
        private async Task<int> GetThenLeastExpensivePlanAsync(TenantPlanPricingOption plan1_option, TenantPlanPricingOption plan2_option)
        {
            var plan1_cycle = await _tenantPricingCycleService.GetByIdAsync(plan1_option.TenantPricingCycleId);
            var plan2_cycle = await _tenantPricingCycleService.GetByIdAsync(plan2_option.TenantPricingCycleId);
            if (plan1_cycle == null || plan2_cycle == null) throw new ArgumentException("Pricing cycle not found");

            if (plan1_cycle.PeriodUnit == plan2_cycle.PeriodUnit)
            {
                return plan1_option.Price < plan2_option.Price ? plan1_option.TenantPlanId : plan2_option.TenantPlanId;

            }
            else
            {
                decimal Plan1PricePerHour = 0;
                decimal Plan2PricePerHour = 0;
                enTenantPricingCycle plan1_billingCycle = (enTenantPricingCycle)Enum.Parse(typeof(enTenantPricingCycle), plan1_cycle.PeriodUnit, true);
                enTenantPricingCycle plan2_billingCycle = (enTenantPricingCycle)Enum.Parse(typeof(enTenantPricingCycle), plan2_cycle.PeriodUnit, true);

                switch (plan1_billingCycle)
                {
                    // set a new Price to hours unit for the first plan
                    case enTenantPricingCycle.hours:
                        Plan1PricePerHour = plan1_option.Price;
                        break;
                    case enTenantPricingCycle.days:
                        Plan1PricePerHour = plan1_option.Price / 24;
                        break;
                    case enTenantPricingCycle.weeks:
                        Plan1PricePerHour = plan1_option.Price / (24 * 7);
                        break;
                    case enTenantPricingCycle.months:
                        Plan1PricePerHour = plan1_option.Price / (24 * 30);
                        break;
                    case enTenantPricingCycle.years:
                        Plan1PricePerHour = plan1_option.Price / (24 * 365);
                        break;





                }

                switch (plan2_billingCycle)
                {
                    // set a new Price to hours unit for the second plan
                    case enTenantPricingCycle.hours:
                        Plan2PricePerHour = plan2_option.Price;
                        break;
                    case enTenantPricingCycle.days:
                        Plan2PricePerHour = plan2_option.Price / 24;
                        break;
                    case enTenantPricingCycle.weeks:
                        Plan2PricePerHour = plan2_option.Price / (24 * 7);
                        break;
                    case enTenantPricingCycle.months:
                        Plan2PricePerHour = plan2_option.Price / (24 * 30);
                        break;
                    case enTenantPricingCycle.years:
                        Plan2PricePerHour = plan2_option.Price / (24 * 365);
                        break;

                }

                return Plan1PricePerHour < Plan2PricePerHour ? plan1_option.TenantPlanId : plan2_option.TenantPlanId;
            }
        }

        public async Task<bool> UpdateTenantPlanWithDependancies(DtoAddNewTenantPlan plan)
        {
        


            if (plan.TenantPlanBenefits.Count() == 0 || plan.TenantPlanPermissions.Count() == 0 || plan.TenantPricingOptions.Count() == 0||(plan.TenantPlan.HasFreeTryOption && plan.TenantFreePlan == null)) throw new ArgumentException();
            if (plan.TenantFreePlan != null) plan.TenantFreePlan.TenantId = _tenantIdProvider.TenantId;


            var freeplan = plan.TenantFreePlan == null ? null : _tenantFreePlanService.GetEntity(plan.TenantFreePlan);


            return await _tenantPlanRepository.UpdateTenantPlan(FromDto(plan.TenantPlan),
                   plan.TenantPlanPermissions.Select(d => _tenantPlanPermissionServices.GetEntity(d)),
                   plan.TenantPlanBenefits.Select(d => _tenantPlanBenifestServices.GetEntity(d)),
                   plan.TenantPricingOptions.Select(d => _tenantPricingOptionServices.GetEntity(d)),
                   freeplan);
        }
        public async Task<bool> IsTenantPlanNameExist(string name)
        {
            var tenantPlan = await _tenantPlanRepository.FindByNameAsync(name);
            return tenantPlan != null;
        }
        public async Task<IEnumerable<DtoTenantPlan>> GetAllWithDependenciesAsync()
        {
            var plans = await _tenantPlanRepository.GetAllWithDependenciesAsync();
            var result = plans.Select(plan => ToDto(plan));

            return result;
        }
        public override Task<bool> DeleteAsync(int id)
        {


            return base.DeleteAsync(id);
        }
        public async Task<IEnumerable<DtoTenantPlan>> GetAllWithDependenciesIgnoreQuerryAsync(string TenantName)
        {
            var Tenant = await _tenantRepo.GetByNameAsync(TenantName);
            if (Tenant == null) throw new ArgumentException("Tenant Name is not relating to any tenant ");
           var plans= await _tenantPlanRepository.GetAllWithDependenciesIgnoreQuerryAsync(Tenant);
            var result = plans.Select(plan =>ToDto(plan)
      );

            return result;
        }
        public async Task<DtoTenantPlan> GetSingleWithDependenciesIgnoreQuerryAsync(string TenantName, string PlanName)
          {


            var Tenant = await _tenantRepo.GetByNameAsync(TenantName);
            if (Tenant == null) throw new ArgumentException("Tenant Name is not relating to any tenant ");
            var plan = await _tenantPlanRepository.GetSingleWithDependenciesIgnoreQuerryAsync(Tenant, PlanName);
            if (plan == null) throw new ArgumentException("Plan  not found ");
            var result = ToDto(plan);
   

            return result;

        }
        public async Task<IEnumerable<DtoTenantPlan>> GetAllPlatformPlansWithDependenciesAsync()
        {

           return await GetAllWithDependenciesIgnoreQuerryAsync(_platformInfo.TenantName);
        }
        public async Task<DtoTenantPlan?> GetSinglePlatformPlanWithDependenciesAsync(string PlanName)
        {
            return await GetSingleWithDependenciesIgnoreQuerryAsync(_platformInfo.TenantName, PlanName);
        }
        public async Task<DtoTenantPlan?> GetSinglePlatformPlanWithDependenciesAsync(int Id)
        {
            var Tenant = await _tenantRepo.GetByNameAsync(_platformInfo.TenantName);
            if (Tenant == null) throw new ArgumentException("Tenant Name is not relating to any tenant ");
            var res= await _tenantPlanRepository.GetSingleWithDependenciesIgnoreQuerryAsync(Tenant, Id);
            return res != null ? ToDto(res) : null;
        }



    }


}
