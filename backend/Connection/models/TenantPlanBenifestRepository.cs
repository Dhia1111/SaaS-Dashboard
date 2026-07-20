using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;


public interface ITenantPlanBenifestRepository : IGenericRepo<TenantPlanBenefit>
{
}
    public class DtoTenantPlanBenefit
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int TenantPlanId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

     public DtoTenantPlan? tenantPlan { get; set; }
    public DtoTenant? Tenant { get; set; }
    
}

namespace Connection
{
   

    public class TenantPlanBenifestRepository
        : GenericRepo<TenantPlanBenefit>,
          ITenantPlanBenifestRepository
    {
        public TenantPlanBenifestRepository(
            SaasDashboardContext context,
            ILogger<TenantPlanBenifestRepository> logger)
            : base(context, logger)
        {
        }
    }
}