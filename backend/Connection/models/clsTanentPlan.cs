using Connection.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Connection.models
{
        public class DtoTenantPlan
        {
            public int Id { get; set; }
            public int DataKey { get; set; }
            public int TenantId { get; set; }

            public string Name { get; set; } = null!;
            public string? Description { get; set; }

            public decimal Price { get; set; }
            public int BillingInterval { get; set; } // enum: monthly, yearly
            public string StripePriceId { get; set; } = null!;
            public bool IsActive { get; set; }

            // Optional navigation DTO
            public DtoTenant? Tenant { get; set; }
        }
        public interface ITenantPlanRepo:IGenericRepo<TenantPlan>
    {


        Task<TenantPlan?> GetByStripePriceIdAsync(string stripePriceId);

        Task<List<TenantPlan>> GetByTenantIdAsync(int tenantId);

        Task<TenantPlan?> GetByIdWithTenantAsync(int id);

    }


        public class clsTenantPlanRepo : GenericRepo<TenantPlan>, ITenantPlanRepo
        {
        public clsTenantPlanRepo(
            SaasDashboardContext context,
            ILogger<GenericRepo<TenantPlan>> logger)
            : base(context, logger)
        {
        }

        public  Task<List<TenantPlan>> GetAllAsync(int dataKey)
        {
            return _context.TenantPlans
                .Where(p => p.DataKey == dataKey)
                .AsNoTracking()
                .ToListAsync();
        }

 

        public  Task<TenantPlan?> GetByStripePriceIdAsync(string stripePriceId)
        {
            return _context.TenantPlans
                .SingleOrDefaultAsync(p => p.StripePriceId == stripePriceId);
        }

        public  Task<List<TenantPlan>> GetByTenantIdAsync(int tenantId)
        {
            return  _context.TenantPlans
                .Where(p => p.TenantId == tenantId && p.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<TenantPlan?> GetByIdWithTenantAsync(int id)
        {
            return _context.TenantPlans
                .Include(p => p.Tenant)
                .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}
