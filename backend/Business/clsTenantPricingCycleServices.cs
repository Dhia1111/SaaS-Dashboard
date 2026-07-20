using Business;
using Business.Config;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ITenantPricingCycleServices:IGenericService<DtoTenantPricingCycle>
{
    Task<bool> IsUnique(string CycleName);
    Task<IReadOnlyList<DtoTenantPricingCycle>> GetAllAsyncWithIgnoreQueryFilter(string TenantName );
    Task<IReadOnlyList<DtoTenantPricingCycle>> GetPlatformPricingCyclesAsync();



}



namespace Business
{
    public class clsTenantPricingCycleServices:GenericService<DtoTenantPricingCycle,TenantPricingCycle>, ITenantPricingCycleServices
    {
        private readonly ITenantPricingCycleRepository _repository;
        private readonly ITenantRepo _tenantRepo;
        private readonly PlatformInfo _platformInfo;
        public clsTenantPricingCycleServices(ITenantPricingCycleRepository repo,
            ILogger<clsTenantPricingCycleServices> logger,
            ITenantRepo tenantRepo, IOptions<PlatformInfo>platforminfo) : base(repo, logger)
        {
            _repository = repo;
            _tenantRepo = tenantRepo;
            _platformInfo = platforminfo.Value;
            

            
        }

        protected override TenantPricingCycle FromDto(DtoTenantPricingCycle dto)
        {
            
            return new TenantPricingCycle
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                CycleName = dto.CycleName,
                Period = dto.Period,
                PeriodUnit = dto.PeriodUnit,
                
            };

        }

        protected override DtoTenantPricingCycle ToDto(TenantPricingCycle Entity)
        {

            return new DtoTenantPricingCycle
            {
                Id = Entity.Id,
                TenantId = Entity.TenantId,
                CycleName = Entity.CycleName,
                Period = Entity.Period,
                PeriodUnit = Entity.PeriodUnit,
                

            };

        }


      public async Task<bool> IsUnique(string CycleName)
        {
            var existingCycle = await _repository.FindAsync(CycleName);
            return existingCycle == null;
        }


        public override Task<int> AddAsync(DtoTenantPricingCycle dto)
        {
            dto.PeriodUnit = dto.PeriodUnit.ToLower() ;

             if (dto.PeriodUnit!=enTenantPricingCycle.hours.ToString().ToLower()&&dto.PeriodUnit != enTenantPricingCycle.days.ToString().ToLower() && dto.PeriodUnit != enTenantPricingCycle.months.ToString().ToLower()&& dto.PeriodUnit != enTenantPricingCycle.years.ToString().ToLower()&& dto.PeriodUnit != enTenantPricingCycle.weeks.ToString().ToLower())
            {
                throw new ArgumentException("PeriodUnit must be either 'hours', 'days', 'months', 'years', or 'weeks'.");
            };

            return base.AddAsync(dto);
        }

        public override Task<bool> UpdateAsync(DtoTenantPricingCycle dto)
        {
            dto.PeriodUnit = dto.PeriodUnit.ToLower();
            if (dto.PeriodUnit != enTenantPricingCycle.hours.ToString().ToLower() && dto.PeriodUnit != enTenantPricingCycle.days.ToString().ToLower() && dto.PeriodUnit != enTenantPricingCycle.months.ToString().ToLower() && dto.PeriodUnit != enTenantPricingCycle.years.ToString().ToLower() && dto.PeriodUnit != enTenantPricingCycle.weeks.ToString().ToLower())
            {
                throw new ArgumentException("PeriodUnit must be either 'hours', 'days', 'months', 'years', or 'weeks'.");
            };
            return base.UpdateAsync(dto);
        }

        public async Task<IReadOnlyList<DtoTenantPricingCycle>> GetAllAsyncWithIgnoreQueryFilter(string TenantName)
        {
            var Tenant = await _tenantRepo.GetByNameAsync(TenantName);
            if (Tenant == null) throw new ArgumentException($"No Tenant with the Name {TenantName}");
            var res= await _repository.GetAllAsyncWithIgnoreQueryFilter(Tenant);
            List<DtoTenantPricingCycle> List = new List<DtoTenantPricingCycle>(res.Count);
            foreach (var item in res)
            {
                List.Add(ToDto(item));

            }

            return List;

        }


      public   async Task<IReadOnlyList<DtoTenantPricingCycle>> GetPlatformPricingCyclesAsync()
        {
            return await GetAllAsyncWithIgnoreQueryFilter(_platformInfo.TenantName);
        }
    }
}
