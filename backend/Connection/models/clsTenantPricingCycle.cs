using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
 

public interface ITenantPricingCycleRepository : IGenericRepo<TenantPricingCycle>
{
    // You can add custom methods specific to TenantPricingCycle here if needed

    Task<TenantPricingCycle> FindAsync(string cycleName);
    public  Task<IReadOnlyList<TenantPricingCycle>> GetAllAsyncWithIgnoreQueryFilter(Tenant Tenant);

}

public class DtoTenantPricingCycle
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string CycleName { get; set; } = null!;

    public int Period { get; set; }// e.g., "Monthly", "Yearly"

    public string PeriodUnit { get; set; } = null!;// in days or in months



}
namespace Connection.models
{
    public class TenantPricingCycleRepository : GenericRepo<TenantPricingCycle>, ITenantPricingCycleRepository
    {
        public TenantPricingCycleRepository(SaasDashboardContext context, ILogger<TenantPricingCycleRepository> repo) : base(context, repo)
        {


        }

        public async Task<TenantPricingCycle> FindAsync(string cycleName)
        {
            return await _context.TenantsPricingCycles.SingleOrDefaultAsync(e => e.CycleName == cycleName);
        }
        public async Task<IReadOnlyList<TenantPricingCycle>> GetAllAsyncWithIgnoreQueryFilter(Tenant Tenant)
        {

            return await _context.TenantsPricingCycles.AsNoTracking().Where(e => e.TenantId == Tenant.TenantId).IgnoreQueryFilters().ToListAsync();


        }
    }
}
