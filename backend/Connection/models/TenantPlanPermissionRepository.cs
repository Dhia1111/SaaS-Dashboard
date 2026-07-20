
using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;



public interface ITenantPlanPermissionRepository : IGenericRepo<TenantPlanPermission>
{
}
    public class DtoTenantPlanPermission
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int TenantPlanId { get; set; }

        public int PermissionId { get; set; }
    }

namespace Connection
{

    public class TenantPlanPermissionRepository
       : GenericRepo<TenantPlanPermission>,
         ITenantPlanPermissionRepository
    {
        public TenantPlanPermissionRepository(
            SaasDashboardContext context,
            ILogger<TenantPlanPermissionRepository> logger)
            : base(context, logger)
        {
        }
    }
}