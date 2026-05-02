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

    public class clsUserSubscriptionRepo :GenericRepo<UserSubscription> ,IUserSubscriptionRepo
    {
       

        public clsUserSubscriptionRepo(
            SaasDashboardContext context,
            ILogger<clsUserSubscriptionRepo> logger):base(context,logger)
        {
            
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



    }
 
}
