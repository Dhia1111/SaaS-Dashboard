using Business;
using Business.Config;
using Connection.models;
using Microsoft.Extensions.Options;
using Quartz;
using SharedDto_Enum;

namespace APIs.BackGroundJobs
{
    public class SetSubscriptionToExpire:IJob
    {
        private readonly ILogger<SetSubscriptionToExpire> _logger;
        private readonly IPlatformSubscriptionService _platformSubscriptionService;
 

        public SetSubscriptionToExpire(
            ILogger<SetSubscriptionToExpire> logger,
            IPlatformSubscriptionService platformSubscriptionService
         
            )
        {
            _logger = logger;
            _platformSubscriptionService = platformSubscriptionService;
            
        }


        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ManageSubscriptionExperition job started at: {Time}", DateTimeOffset.UtcNow);
            try
            {
                // Call the method to manage client subscriptions
                await SetSubscriptionToExpireManager();
                _logger.LogInformation("ManageSubscriptionExperition job completed successfully at: {Time}", DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ManageSubscriptionExperition job failed at: {Time}", DateTimeOffset.UtcNow);
                throw; // Re-throw to let Quartz handle the failure (retry, misfire, etc.)
            }
        }

                  private async Task<bool> SetSubscriptionToExpireManager()
        {
            return await _platformSubscriptionService.setToExpireAllEndEdSubscription();
        }
    }
}
