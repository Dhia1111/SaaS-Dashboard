
using Business;
using ExternalAPI;
public interface IEmailJob
{
    Task SendEmailAsync();
}
namespace APIs.BackGroundJobs
{
    public class EmailBackgroundJob : BackgroundService, IEmailJob
    {
        private readonly ILogger<EmailBackgroundJob> _logger;
        private readonly IEmailService _emailService;
        private readonly IEmailExternalService _emailExternalService;

        public EmailBackgroundJob(ILogger<EmailBackgroundJob> logger, IEmailService emailService, IEmailExternalService emailExternalService)
        {
            _logger = logger;
            _emailService = emailService;
            _emailExternalService = emailExternalService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Simulate email sending
                _logger.LogInformation("Sending email at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken); // Simulate delay for sending email
                //
            }
        }

        public async Task SendEmailAsync()
        {
            // Logic to send email
            IReadOnlyList<DtoEmail>? emailsToSend = await _emailService.GetAllAsync(); // Fetch emails to send from database or queue
          if (emailsToSend == null  )
            {
                _logger.LogInformation("No emails to send at: {time}", DateTime.UtcNow);
                return;
            }
            if (emailsToSend.Count == 0) {
                _logger.LogInformation("No emails to send at: {time}", DateTime.UtcNow);
                return;
            }

            foreach (var email in emailsToSend)
            {
                if(email.IsSent)
                {
                    continue; // Skip already sent emails
                }
                try
                {
                    ExternalAPI.DtoEmail ExternalEmail = new  ExternalAPI.DtoEmail  { 
                    Body=email.Body,
                    Subject=email.Subject,
                    From  =email.From,
                    To =email.To,
                    };
                 await _emailExternalService.SendEmail(ExternalEmail); // Send email using external service
             
                    email.IsSent = true; // Mark email as sent
                    email.SentAt = DateTime.UtcNow; // Set sent time
                    await _emailService.UpdateAsync(email); // Update email status in database
                    _logger.LogInformation("Email sent successfully to: {to} at: {time}", email.To, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to: {to} at: {time}", email.To, DateTime.UtcNow);
                }

                
            }
        }
    }

   
}