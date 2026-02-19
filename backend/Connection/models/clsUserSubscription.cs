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
    public class DtoUserSubscription
    {
        public int Id { get; set; }

        public int DataKey { get; set; }
        public int TenantId { get; set; }
        public int UserId { get; set; }

        public int SubscriptionTypeId { get; set; }

        public int Status { get; set; } // enum GeneralState
        public int BillingInterval { get; set; } // enum PlatformBillingInterval

        public string StripeSubscriptionId { get; set; } = null!;
        public string StripePriceId { get; set; } = null!;

        public string StartDate { get; set; }=null!;
        public string? EndDate { get; set; }
    }
    public interface IUserSubscriptionRepo:IGenericRepo<UserSubscription>
    {


        Task<UserSubscription?> GetByUserIdAsync(int userId);



    }

    public class clsUserSubscriptionRepo : IUserSubscriptionRepo
    {
        private readonly SaasDashboardContext _context;
        private readonly ILogger<clsUserSubscriptionRepo> _logger;

        public clsUserSubscriptionRepo(
            SaasDashboardContext context,
            ILogger<clsUserSubscriptionRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<UserSubscription>> GetAllAsync(int dataKey)
        {
            try
            {
                return await _context.UserSubscriptions.AsNoTracking()
                    .Where(s => s.DataKey == dataKey)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching UserSubscriptions for DataKey {DataKey}", dataKey);
                throw;
            }
        }

        public async Task<UserSubscription?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.UserSubscriptions.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching UserSubscription by Id {Id}", id);
                throw;
            }
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.UserSubscriptions
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching UserSubscription by UserId {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> AddAsync(UserSubscription subscription)
        {
            try
            {
                _context.UserSubscriptions.Add(subscription);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "UserSubscription created with Id {Id}", subscription.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating UserSubscription for UserId {UserId}",
                    subscription.Id);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(UserSubscription subscription)
        {
            try
            {
                _context.UserSubscriptions.Update(subscription);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "UserSubscription updated with Id {Id}", subscription.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating UserSubscription with Id {Id}",
                    subscription.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(UserSubscription subscription)
        {
            try
            {
                _context.UserSubscriptions.Remove(subscription);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "UserSubscription deleted with Id {Id}", subscription.Id);

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting UserSubscription with Id {Id}",
                    subscription.Id);
                return false;
            }
        }
    }
 
}
