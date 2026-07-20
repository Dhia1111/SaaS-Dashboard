
using Quartz;
using ExternalAPI;
using SharedDto_Enum;
namespace APIs.BackGroundJobs
{
    [DisallowConcurrentExecution]  // Prevents multiple instances of this job running simultaneously
    public class SetPaymentToExprierBackgroundJob : IJob  // No need for custom IEmailJob interface
    {
        private readonly ILogger<SetPaymentToExprierBackgroundJob> _logger;
        private readonly IPaymentService _paymentService;
 
        public SetPaymentToExprierBackgroundJob(
            ILogger<SetPaymentToExprierBackgroundJob> logger,
            IPaymentService paymentService
             )
        {
            _logger = logger;
            _paymentService = paymentService;
            
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(" Payments background job started at: " + DateTimeOffset.UtcNow);

            _logger.LogInformation("Payments background job started at: {Time}",
              DateTimeOffset.UtcNow);

            await SetPaymentsToExpier();
            
            _logger.LogInformation("SetPaymentsToExpier background job completed successfully at: {Time}", DateTimeOffset.UtcNow);




        }

        private async Task SetPaymentsToExpier()
        {
            var pendingPayments = await _paymentService.GetAllByStatusWithIgnoreQueryFiltersAsync(enGeneralState.Pending);

            foreach(var payment in pendingPayments)
            {

                await setToExperSinglePaymentAsync(payment);


            }
           

         }

        private async Task setToExperSinglePaymentAsync(DtoPayment payment)
        {
            // Skip if already sent (double-check)
            if (payment.PaymentStatus!=enGeneralState.Pending)
            {
                _logger.LogDebug("Payment {id} does not have pendding status, skipping", payment.Id);
                return;
            }
            int PeriodToSetExpire = 30; //22 min
            if (DateTime.Parse(payment.CreatedAt).AddMinutes(PeriodToSetExpire)<DateTime.UtcNow)
            {
               
                return;
            }
            payment.PaymentStatus = enGeneralState.Expired;
            
            await _paymentService.UpdateAsync(payment);

            
        }
    }
}