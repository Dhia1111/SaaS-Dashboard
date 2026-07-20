using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



public interface ITenantPricingOptionRepository : IGenericRepo<TenantPlanPricingOption>
{
    public Task<IEnumerable<TenantPlanPricingOption>> GetAllPlanPricingOptionsWithFilterIgnoreAsync(int TenatId);

}
public class DtoTenantPricingOption
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int TenantPricingCycleId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;


    public bool IsActive { get; set; } = true;

        public int TenantPlanId { get; set; }
    }

namespace Connection
{
  

    public class TenantPricingOptionRepository
        : GenericRepo<TenantPlanPricingOption>,
          ITenantPricingOptionRepository
    {
        public TenantPricingOptionRepository(
            SaasDashboardContext context,
            ILogger<TenantPricingOptionRepository> logger)
            : base(context, logger)
        {
        }


        public async  Task<IEnumerable<TenantPlanPricingOption>> GetAllPlanPricingOptionsWithFilterIgnoreAsync(int TenantId)
        {


           return await _context.TenantsPricingOptions.IgnoreQueryFilters().Where(p=>p.TenantId==TenantId).ToListAsync();


         }


    }
}