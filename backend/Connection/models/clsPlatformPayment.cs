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
        public int DataKey { get; set; }
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

    public class clsPlatformPaymentRepo : IPlatformPaymentRepo
    {
        private readonly SaasDashboardContext _context;
        private readonly ILogger<clsPlatformPaymentRepo> _logger;

        public clsPlatformPaymentRepo(
            SaasDashboardContext context,
            ILogger<clsPlatformPaymentRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<PlatformPayment>> GetAllAsync(int dataKey)
        {
            try
            {
                return await _context.PlatformPayments.AsNoTracking()
                    .Where(p => p.DataKey == dataKey)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching PlatformPayments for DataKey {DataKey}", dataKey);
                throw;
            }
        }

        public async Task<PlatformPayment?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.PlatformPayments.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching PlatformPayment by Id {Id}", id);
                throw;
            }
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

        public async Task<bool> AddAsync(PlatformPayment payment)
        {
            try
            {
                _context.PlatformPayments.Add(payment);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "PlatformPayment created with Id {Id} for TenantId {TenantId}",
                    payment.Id,
                    payment.TenantId);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating PlatformPayment for TenantId {TenantId}",
                    payment.TenantId);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(PlatformPayment payment)
        {
            try
            {
                _context.PlatformPayments.Update(payment);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "PlatformPayment updated with Id {Id}",
                    payment.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating PlatformPayment with Id {Id}",
                    payment.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(PlatformPayment payment)
        {
            try
            {
                _context.PlatformPayments.Remove(payment);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "PlatformPayment deleted with Id {Id}",
                    payment.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting PlatformPayment with Id {Id}",
                    payment.Id);
                return false;
            }
        }
    }
}
