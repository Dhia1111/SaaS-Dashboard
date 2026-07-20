

using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

public class DtoTenantFreePlan {
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int TenantPlanId { get; set; }
 
    public int CycleId { get; set; }

    public string StartAt { get; set; } = null!;

    public string EndAt { get; set; } = null!;



}

public interface ITenantFreePlanRepo:IGenericRepo<TenantFreePlan> {



    public Task<TenantFreePlan?> GetByPlanIdAsync(int planId);


}


namespace Connection.models
{
    public class clsTenantFreePlanRepo:GenericRepo<TenantFreePlan>,ITenantFreePlanRepo
    {
        public clsTenantFreePlanRepo(SaasDashboardContext context,ILogger<clsTenantFreePlanRepo> logger) : base(context, logger) { 
        


        }
        public async Task<TenantFreePlan?> GetByPlanIdAsync(int planId)
        {
           return await _context.TenantsFreePlans.SingleOrDefaultAsync(e=>e.TenantPlanId==planId);

        }



    }
}
