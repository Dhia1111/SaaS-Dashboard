using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System.Security;

public class DtoAddNewTenantPlan
{
    public DtoTenantPlan TenantPlan { get; set; } = null!;
    public DtoTenantFreePlan? TenantFreePlan { get; set; }

    public IEnumerable<DtoTenantPlanBenefit> TenantPlanBenefits { get; set; } = null!;
    public IEnumerable<DtoTenantPricingOption> TenantPricingOptions { get; set; } = null!;
    public IEnumerable<DtoTenantPlanPermission> TenantPlanPermissions { get; set; } = null!;

}


public class DtoUpdateTenantPlanWithDependencies
{

    public DtoTenantPlan TenantPlan { get; set; } = null!;
    public DtoTenantFreePlan? TenantFreePlan { get; set; }
    public IEnumerable<DtoTenantPlanBenefit> NewTenantPlanBenefits { get; set; } = null!;
    public IEnumerable<DtoTenantPricingOption> NewTenantPricingOptions { get; set; } = null!;
    public IEnumerable<DtoTenantPlanPermission> NewTenantPlanPermissions { get; set; } = null!;
    public IEnumerable<DtoTenantPlanBenefit> RemoveTenantPlanBenefits { get; set; } = null!;
    public IEnumerable<DtoTenantPricingOption> RemoveTenantPricingOptions { get; set; } = null!;
    public IEnumerable<DtoTenantPlanPermission> RemoveTenantPlanPermissions { get; set; } = null!;
   


}
public interface ITenantPlanRepository : IGenericRepo<TenantPlan>
{

    Task<int> AddNewTenantPlan(TenantPlan tenantPlan,
        List<TenantPlanPermission> planPermission,
        List<TenantPlanBenefit> planBenifest,
        List<TenantPlanPricingOption> pricingOption,TenantFreePlan tenantFreePlan=null);

    Task<bool> UpdateTenantPlan(TenantPlan tenantPlan,
      IEnumerable<TenantPlanPermission> planPermission,
      IEnumerable<TenantPlanBenefit> planBenifest,
      TenantFreePlan tenantFreePlan=null);

    Task<TenantPlan?> FindByNameAsync(string name);

    Task<IEnumerable<TenantPlan>> GetAllWithDependenciesAsync();
    public  Task<IEnumerable<TenantPlan>> GetAllWithDependenciesIgnoreQuerryAsync(Tenant Tenant);
    public  Task<TenantPlan?> GetSingleWithDependenciesIgnoreQuerryAsync(Tenant Tenant, string PlanName);
    public Task<TenantPlan?> GetSingleWithDependenciesIgnoreQuerryAsync(int Tenant, int PlanId);
    public Task<TenantPlan?> GetSingleWithDependenciesAsync( string PlanName);
    public Task<TenantPlan?> GetSingleWithDependenciesAsync( int PlanId);


    public Task<TenantPlan?> GetSingleWithDependenciesIgnoreQuerryAsync(Tenant Tenant, int PlanId);


}

public class DtoTenantPlan
    {
     public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool HasFreeTryOption { get; set; } = false;
    public DtoTenant? Tenant { get; set; }
    public IEnumerable<DtoTenantPlanPermission>? Permissions { get; set; }
    public IEnumerable<DtoTenantPlanBenefit>? Benefits { get; set; }
    public IEnumerable<DtoTenantPricingOption>? PlanPricingOptions { get; set; }
    public DtoTenantFreePlan?TenantFreePlan { get; set; }
    public int GradeLevel { get; set; } 


}

namespace Connection
{


    public class TenantPlanRepository : GenericRepo<TenantPlan>, ITenantPlanRepository
    {
        public TenantPlanRepository(SaasDashboardContext context, ILogger<TenantPlanRepository> logger) : base(context, logger)
        {
        }

        public async Task<int> AddNewTenantPlan(TenantPlan tenantPlan,
        List<TenantPlanPermission> planPermission,
        List<TenantPlanBenefit> PlanBenefits,
        List<TenantPlanPricingOption> pricingOptions,TenantFreePlan TenantFreePlan=null)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                await _context.TenantPlans.AddAsync(tenantPlan);
                await _context.SaveChangesAsync();

                if (TenantFreePlan != null)
                {
                    TenantFreePlan.TenantPlanId = tenantPlan.Id;
                    await _context.TenantsFreePlans.AddAsync(TenantFreePlan);

                }

                foreach (var permission in planPermission)
                {
                    permission.TenantPlanId = tenantPlan.Id;
                    Console.WriteLine($"tpId {permission.TenantPlanId}");

                }
                foreach (var benfit in PlanBenefits)
                {
                    benfit.TenantPlanId = tenantPlan.Id;
                    Console.WriteLine($"tpId {benfit.TenantPlanId}");


                }
                Console.WriteLine(
    PlanBenefits.First().GetType().FullName);
                foreach (var priceOption in pricingOptions)
                {
                    priceOption.TenantPlanId = tenantPlan.Id;
                    Console.WriteLine($"tpId {priceOption.TenantPlanId}");

                }

                await _context.TenantsPlansPermissions.AddRangeAsync(planPermission);
                await _context.TenantsPlansBenifests.AddRangeAsync(PlanBenefits);
                await _context.TenantsPricingOptions.AddRangeAsync(pricingOptions);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return tenantPlan.Id;


            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }



        }


        public async Task<bool> UpdateTenantPlan(TenantPlan tenantPlan,
        IEnumerable<TenantPlanPermission> planPermission,
        IEnumerable<TenantPlanBenefit> PlanBenefits,
        TenantFreePlan? tenantFreePlan=null)
        {

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.TenantPlans.Update(tenantPlan);
                if (tenantFreePlan != null)
                {
                    _context.TenantsFreePlans.Update(tenantFreePlan);
                }
                if (!tenantPlan.HasFreeTryOption)
                {
                    if(tenantFreePlan == null)
                    {
                        tenantFreePlan= await _context.TenantsFreePlans.SingleOrDefaultAsync(item=>item.TenantPlanId==tenantPlan.Id);
                    }
                    if(tenantFreePlan!=null) _context.TenantsFreePlans.Remove(tenantFreePlan);

                }
                await _context.TenantsPlansPermissions.Where(p=>p.TenantPlanId==tenantPlan.Id).ExecuteDeleteAsync();
                await  _context.TenantsPlansBenifests.Where(p => p.TenantPlanId == tenantPlan.Id).ExecuteDeleteAsync();

                var CurpriceOptions = _context.TenantsPricingOptions.Where(e => e.TenantPlanId == tenantPlan.Id).ToDictionary(e=>e.Id,e=>e);


                await _context.TenantsPlansPermissions.AddRangeAsync(planPermission);
                await  _context.TenantsPlansBenifests.AddRangeAsync(PlanBenefits);
                
                
                 await  _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;


            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }



        }

        public async Task<TenantPlan?> FindByNameAsync(string name)
        {
            return await _context.TenantPlans.SingleOrDefaultAsync(tp => tp.Name == name);

        }

        public async Task<IEnumerable<TenantPlan>> GetAllWithDependenciesAsync()
        {
            var plan = await _context.TenantPlans.AsNoTracking()
         .AsSplitQuery().Include(x=>x.TenantFreePlan)
         .Include(x => x.Permissions)
         .Include(x => x.Benefits)
         .Include(x => x.PricingOptions)
         .ToListAsync();
            return plan;

        }


        public async Task<TenantPlan?>GetSingleWithDependenciesIgnoreQuerryAsync(Tenant Tenant,string PlanName)
        {

            var plan = await _context.TenantPlans.AsNoTracking().IgnoreQueryFilters().
                Include(x => x.Benefits)
                .Include(x => x.TenantFreePlan)
                .Include(x => x.PricingOptions)
                .Include(x => x.Permissions)
         .SingleOrDefaultAsync(p => p.TenantId == Tenant.TenantId && p.Name == PlanName);

            return plan;

        }


        public async Task<TenantPlan?> GetSingleWithDependenciesIgnoreQuerryAsync(Tenant Tenant, int PlanId)
        {

            var plan = await _context.TenantPlans.AsNoTracking().IgnoreQueryFilters().
                Include(x=>x.Benefits)
                .Include(x=> x.TenantFreePlan)
                .Include(x=>x.PricingOptions)
                .Include(x=>x.Permissions)
         .SingleOrDefaultAsync(p => p.TenantId == Tenant.TenantId && p.Id == PlanId);

            return plan;

        }

        public async Task<TenantPlan?> GetSingleWithDependenciesIgnoreQuerryAsync(int TenantId, int PlanId)
        {

            var plan = await _context.TenantPlans.AsNoTracking().IgnoreQueryFilters().
                Include(x => x.Benefits)
                .Include(x => x.TenantFreePlan)
                .Include(x => x.PricingOptions)
                .Include(x => x.Permissions)
         .SingleOrDefaultAsync(p => p.TenantId == TenantId && p.Id == PlanId);

            return plan;

        }

        public async Task<TenantPlan?> GetSingleWithDependenciesAsync(int PlanId)
        {

            var plan = await _context.TenantPlans.AsNoTracking().
                Include(x => x.Benefits)
                .Include(x => x.TenantFreePlan)
                .Include(x => x.PricingOptions)
                .Include(x => x.Permissions)
         .SingleOrDefaultAsync(p =>  p.Id == PlanId);

            return plan;

        }

        public async Task<TenantPlan?> GetSingleWithDependenciesAsync( string PlanName)
        {

            var plan = await _context.TenantPlans.AsNoTracking().
                Include(x => x.Benefits)
                .Include(x => x.TenantFreePlan)
                .Include(x => x.PricingOptions)
                .Include(x => x.Permissions)
         .SingleOrDefaultAsync(p =>  p.Name ==PlanName );

            return plan;

        }

        public async Task<IEnumerable<TenantPlan>> GetAllWithDependenciesIgnoreQuerryAsync(Tenant Tenant)
        {

            var plan = await _context.TenantPlans.AsNoTracking().IgnoreQueryFilters().AsSplitQuery().
                Where(p=>p.TenantId==Tenant.TenantId).Include(x => x.TenantFreePlan)
         .Include(x => x.Permissions)
         .Include(x => x.Benefits)
         .Include(x => x.PricingOptions)
         .ToListAsync();

            return plan;

        }
    }
}