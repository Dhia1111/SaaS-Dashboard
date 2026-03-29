
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net.Mail;

namespace ExternalAPI
{
    public sealed class EmailServiceUnavailableException : Exception
    {
        public EmailServiceUnavailableException(Exception innerException)
            : base("Email service is unavailable.", innerException)
        {
        }
    }
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }

    public interface IEmailSettingsFactory
    {
        EmailSettings CreateSettings();
    }

    public class EnvironmentEmailSettingsFactory : IEmailSettingsFactory
    {
        private readonly ILogger<EnvironmentEmailSettingsFactory> _logger;

        public EnvironmentEmailSettingsFactory(ILogger<EnvironmentEmailSettingsFactory> logger)
        {
            _logger = logger;
        }

        public EmailSettings CreateSettings()
        {
            var settings = new EmailSettings();

            // Read and validate all settings
            settings.Host = GetRequiredSetting("EMAIL_HOST"); 
            settings.Port = GetAndValidatePort();
            settings.Email = GetRequiredSetting("EMAIL");
            settings.Password = GetRequiredSetting("EMAIL_PASSWORD");
            settings.EnableSsl = GetSslSetting();

            _logger.LogInformation("Email settings loaded successfully for user: {Username}", settings.Email);

            return settings;
        }

        private string GetRequiredSetting(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogError("Required environment variable {VariableName} is not set or empty", variableName);
                throw new InvalidOperationException($"Environment variable '{variableName}' is required but not set");
            }

            return value;
        }

        private int GetAndValidatePort()
        {
            var portString = Environment.GetEnvironmentVariable("EMAIL_PORT");

            if (string.IsNullOrWhiteSpace(portString))
            {
                _logger.LogError("EMAIL_PORT environment variable is not set or empty");
                throw new InvalidOperationException("EMAIL_PORT environment variable is required but not set");
            }

            if (!int.TryParse(portString, out int port))
            {
                _logger.LogError("EMAIL_PORT value '{PortString}' is not a valid integer", portString);
                throw new InvalidOperationException($"EMAIL_PORT must be a valid integer. Received: {portString}");
            }

            if (port < 1 || port > 65535)
            {
                _logger.LogError("EMAIL_PORT {Port} is outside valid range (1-65535)", port);
                throw new InvalidOperationException($"EMAIL_PORT must be between 1 and 65535. Received: {port}");
            }

            return port;
        }

        private bool GetSslSetting()
        {
            var sslString = Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL");

            if (string.IsNullOrWhiteSpace(sslString))
            {
                _logger.LogInformation("EMAIL_ENABLE_SSL not set, defaulting to true");
                return true;
            }

            if (bool.TryParse(sslString, out bool enableSsl))
            {
                return enableSsl;
            }

            _logger.LogWarning("Invalid EMAIL_ENABLE_SSL value '{SslString}', defaulting to true", sslString);
            return true;
        }
    }

    public class DtoEmail
    {
        public string Subject { get; set; } = null!;
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public bool IsBodyAnHtml { get; set; }

        public string Body { get; set; } = null!;
    }

    public interface IEmailExternalService
    {

        public Task SendEmail(DtoEmail Dto);

    }

    public class clsExternalEmailService : IEmailExternalService
    {
        private readonly ILogger<clsExternalEmailService> _logger;
        private readonly SmtpClient _smptClient;
      public clsExternalEmailService(ILogger<clsExternalEmailService> loger, SmtpClient smptClient)
        {
            _logger = loger;
            _smptClient = smptClient;
        }

        public async Task SendEmail(DtoEmail dto)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(dto.From),
                Subject = dto.Subject,
                Body = dto.Body,
                IsBodyHtml = dto.IsBodyAnHtml
            };

            mailMessage.To.Add(dto.To);

            try
            {
                await _smptClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email to {To}", dto.To);
                throw;                             // preserve truth
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error while sending email");
                throw;
            }
        }

    }
      

    }



