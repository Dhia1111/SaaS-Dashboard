using Connection.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public class DtoPlatformSubscription
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public int PlatformPlanId { get; set; }

        public int Status { get; set; } // enum

        public string StripeSubscriptionId { get; set; } = null!;

        public string StartedAt { get; set; }= null!;
        public string? EndsAt { get; set; }=null!;

        public DtoTenant Tenant { get; set; } = null!;
        public DtoPlatformPlan PlatformPlan { get; set; } = null!;
    }

    public interface IPlatformSubscriptionRepo:IGenericRepo<PlatformSubscription>
    {
        Task<PlatformSubscription?> GetByTenantIdAsync(int tenantId);

        Task<PlatformSubscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId);

        Task<PlatformSubscription?> GetActiveByTenantIdAsync(int tenantId);
    }
 
    
        public class clsPlatformSubscriptionRepo
            : GenericRepo<PlatformSubscription>, IPlatformSubscriptionRepo
        {
            public clsPlatformSubscriptionRepo(
                SaasDashboardContext context,
                ILogger<GenericRepo<PlatformSubscription>> logger)
                : base(context, logger)
            {
            }

            public async Task<PlatformSubscription?> GetByTenantIdAsync(int tenantId)
            {
                try
                {
                    return await _context.PlatformSubscriptions
                        .Include(s => s.PlatformPlan)
                        .Include(s => s.Tenant)
                        .SingleOrDefaultAsync(s => s.TenantId == tenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error fetching PlatformSubscription by TenantId {TenantId}",
                        tenantId);

                    throw;
                }
            }

            public async Task<PlatformSubscription?> GetActiveByTenantIdAsync(int tenantId)
            {
                try
                {
                    return await _context.PlatformSubscriptions
                        .Include(s => s.PlatformPlan)
                        .SingleOrDefaultAsync(s =>
                            s.TenantId == tenantId &&
                            s.Status == (int)GeneralState.Active &&
                            s.EndsAt == null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error fetching ACTIVE PlatformSubscription for TenantId {TenantId}",
                        tenantId);

                    throw;
                }
            }

            public async Task<PlatformSubscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId)
            {
                try
                {
                    return await _context.PlatformSubscriptions
                        .Include(s => s.Tenant)
                        .Include(s => s.PlatformPlan)
                        .SingleOrDefaultAsync(s =>
                            s.StripeSubscriptionId == stripeSubscriptionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error fetching PlatformSubscription by StripeSubscriptionId {StripeSubscriptionId}",
                        stripeSubscriptionId);

                    throw;
                }
            }
        }
    

}
