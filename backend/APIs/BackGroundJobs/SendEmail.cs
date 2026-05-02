using Business;
using Quartz;
using ExternalAPI;
using Microsoft.Extensions.Logging;

namespace APIs.BackGroundJobs
{
    [DisallowConcurrentExecution]  // Prevents multiple instances of this job running simultaneously
    public class EmailBackgroundJob : IJob  // No need for custom IEmailJob interface
    {
        private readonly ILogger<EmailBackgroundJob> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailExternalService _emailExternalService;

        public EmailBackgroundJob(
            ILogger<EmailBackgroundJob> logger,
            IEmailService emailService,
            IEmailExternalService emailExternalService)
        {
            _logger = logger;
            _emailService = emailService;
            _emailExternalService = emailExternalService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Email background job started at: " + DateTimeOffset.Now);
 
            _logger.LogInformation("Email background job started at: {Time}",
                DateTimeOffset.Now);

            try
            {
                await SendEmailAsync();
                _logger.LogInformation("Email background job completed successfully at: {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email background job failed at: {Time}", DateTimeOffset.Now);
                throw; // Re-throw to let Quartz handle the failure (retry, misfire, etc.)
            }
        }

        private async Task SendEmailAsync()
        {
            // Fetch only pending emails (not sent) with pagination
            var emailsToSend = await _emailService.GetAllAsync();

            if (emailsToSend == null || emailsToSend.Count == 0)
            {
                _logger.LogInformation("No pending emails to send at: {Time}", DateTime.UtcNow);
                return;
            }

            _logger.LogInformation("Found {Count} pending emails to process", emailsToSend.Count);

            // Process emails in parallel with controlled concurrency
  
            foreach (var email in emailsToSend)
            {
                await ProcessSingleEmailAsync(email);
            }

         }

        private async Task ProcessSingleEmailAsync(DtoEmail email)
        {
            // Skip if already sent (double-check)
            if (email.IsSent)
            {
                _logger.LogDebug("Email {EmailId} already marked as sent, skipping", email.Id);
                return;
            }
            // Implement retry logic
            var maxRetries = 3;
            //var retryDelay = TimeSpan.FromSeconds(5);

        
                try
                {
                    // Convert to external DTO
                    var externalEmail = new ExternalAPI.DtoEmail
                    {
                        Body = email.Body,
                        Subject = email.Subject,
                        From = email.From,
                        To = email.To,
                        IsBodyAnHtml = email.IsBodyAnHtml,
                    };

                    // Send email using external service
                    Console.WriteLine($"Started {DateTime.UtcNow}");

                    await _emailExternalService.SendEmailAsync(externalEmail);

                    Console.WriteLine($"ended {DateTime.UtcNow}");

                    email.IsSent = true;
                    await _emailService.UpdateAsync(email);
 
                }
                catch (Exception ex)
                {


                    _logger.LogError(ex, "Failed to send email to: {To} after {MaxRetries} attempts. Error: {Error}",
                        email.To, maxRetries, ex.Message);

                }
            
        }
    }
}