
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
 

namespace ExternalAPI
{

    public static class ExternalAPIDependencyInjection
    {
        


        public static IServiceCollection AddExternalAPIDependencies(this IServiceCollection services)
        {
            services.AddLogging();
            // Register the settings factory
            services.AddSingleton<IEmailSettingsFactory, EnvironmentEmailSettingsFactory>();

            // Create and register email settings
            using var serviceProvider = services.BuildServiceProvider();
            var settingsFactory = serviceProvider.GetRequiredService<IEmailSettingsFactory>();
            var emailSettings = settingsFactory.CreateSettings();

            services.AddSingleton(emailSettings);

            // Register SmtpClient
            services.AddScoped<SmtpClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<EmailSettings>();
                var logger = serviceProvider.GetRequiredService<ILogger<SmtpClient>>();

                try
                {
                    var client = new SmtpClient(settings.Host, settings.Port)
                    {
                        EnableSsl = settings.EnableSsl,
                        Credentials = new NetworkCredential(settings.Email, settings.Password)
                    };

                    logger.LogDebug("SMTP client created for {Host}:{Port}", settings.Host, settings.Port);
                    return client;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to create SMTP client for {Host}:{Port}", settings.Host, settings.Port);
                    throw new InvalidOperationException($"Failed to create SMTP client: {ex.Message}", ex);
                }
            });
            services.AddScoped<IEmailExternalService, clsExternalEmailService>();
            return services;
        }
   
    
    }

}
