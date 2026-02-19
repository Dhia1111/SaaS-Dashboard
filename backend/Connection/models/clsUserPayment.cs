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
    public class DtoUserPayment
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public int DataKey { get; set; }

        public string UserReferenceId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;

        public string StripePaymentIntentId { get; set; } = null!;

        // ISO 8601 string (ex: 2026-02-17T12:30:00Z)
        public string PaidAt { get; set; } = null!;
    }
    public interface IUserPaymentRepo:IGenericRepo<UserPayment>
    {

        Task<IReadOnlyList<UserPayment>> GetByTenantAsync(int tenantId);

        Task<UserPayment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId);

        Task<IReadOnlyList<UserPayment>> GetByUserReferenceAsync(
            int tenantId,
            string userReferenceId);
    }
    public class clsUserPaymentRepo
            : GenericRepo<UserPayment>, IUserPaymentRepo
        {
            public clsUserPaymentRepo(
                SaasDashboardContext context,
                ILogger<GenericRepo<UserPayment>> logger)
                : base(context, logger)
            {
            }

            public override async Task<IReadOnlyList<UserPayment>> GetAllAsync(int dataKey)
            {
                try
                {
                    return await _context.UserPayments
                        .AsNoTracking()
                        .Where(p => p.DataKey == dataKey)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error fetching UserPayments for DataKey {DataKey}", dataKey);
                    throw;
                }
            }

            public async Task<IReadOnlyList<UserPayment>> GetByTenantAsync(int tenantId)
            {
                try
                {
                    return await _context.UserPayments
                        .AsNoTracking()
                        .Where(p => p.TenantId == tenantId)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error fetching UserPayments for TenantId {TenantId}", tenantId);
                    throw;
                }
            }

            public async Task<UserPayment?> GetByStripePaymentIntentIdAsync(
                string stripePaymentIntentId)
            {
                try
                {
                    return await _context.UserPayments
                        .AsNoTracking()
                        .SingleOrDefaultAsync(p =>
                            p.StripePaymentIntentId == stripePaymentIntentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error fetching UserPayment by StripePaymentIntentId {StripeId}",
                        stripePaymentIntentId);
                    throw;
                }
            }

            public async Task<IReadOnlyList<UserPayment>> GetByUserReferenceAsync(
                int tenantId,
                string userReferenceId)
            {
                try
                {
                    return await _context.UserPayments
                        .AsNoTracking()
                        .Where(p =>
                            p.TenantId == tenantId &&
                            p.UserReferenceId == userReferenceId)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error fetching UserPayments for TenantId {TenantId} and UserReferenceId {UserRef}",
                        tenantId, userReferenceId);
                    throw;
                }
            }
        }
   

}


