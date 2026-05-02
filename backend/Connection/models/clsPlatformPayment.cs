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
    public class DtoPlatformPayment
    {
        public int Id { get; set; }

        public int PlatformPlanId { get; set; } 
        public int TenantId { get; set; }
        public int UserId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;

        public int Status { get; set; } // enum GeneralState
        public int BillingInterval { get; set; } // enum PlatformBillingInterval

        public string StripePaymentIntentId { get; set; } = null!;
        public string StripePriceId { get; set; } = null!;

        public string PaymentDate { get; set; } = null!;
    }
    public interface IPlatformPaymentRepo:IGenericRepo<PlatformPayment>
    {


        Task<IReadOnlyList<PlatformPayment>> GetByTenantIdAsync(int tenantId);



    }

    public class clsPlatformPaymentRepo :GenericRepo<PlatformPayment> ,IPlatformPaymentRepo
    {
       
        public clsPlatformPaymentRepo(
            SaasDashboardContext context,
            ILogger<clsPlatformPaymentRepo> logger):base(context,logger)
        {
           
        }



        public async Task<IReadOnlyList<PlatformPayment>> GetByTenantIdAsync(int tenantId)
        {
            try
            {
                return await _context.PlatformPayments
                    .AsNoTracking()
                    .Where(p => p.TenantId == tenantId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching PlatformPayments for TenantId {TenantId}", tenantId);
                throw;
            }
        }



    }
}
