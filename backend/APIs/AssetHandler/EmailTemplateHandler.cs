 using Microsoft.AspNetCore.Hosting;
using Business;
namespace APIs.AssetHandler
{
  
    public class EmailTemplateHandler : IEmailTemplateHandler
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailTemplateHandler> _logger;

        public EmailTemplateHandler(IWebHostEnvironment env, ILogger<EmailTemplateHandler> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> CreateTemplate(string code)
        {
            try
            {
                // 1. Locate the file in wwwroot
                string filePath = Path.Combine(_env.WebRootPath, "EmailTemplate.html");

                if (!File.Exists(filePath))
                {
                    _logger.LogError("Email template not found at: {Path}", filePath);
                    throw new FileNotFoundException("The email template file is missing.");
                }

                // 2. Read the file content
                string htmlContent = await File.ReadAllTextAsync(filePath);

                // 3. Replace the placeholder
                // Using .Replace is safer than string.Format for raw HTML
                return htmlContent.Replace("{verificationCode}", code).Replace("{DateTime.Now.Year}",DateTime.Now.Year.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error generating email template");
                // Return a fallback plain-text version so the system doesn't crash
                return $"Your verification code is: {code} (Valid for 60s)";
            }
        }
    }
}