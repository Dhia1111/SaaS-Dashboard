
using APIs.AssetHandler;
using APIs.Hashing;
using Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
 

namespace ExternalAPI
{

    public static class APIDependencyInjection
    {
        


        public static IServiceCollection AddAPIDependencies(this IServiceCollection services)
        {
             services.AddScoped<IEmailTemplateHandler, EmailTemplateHandler>();
            services.AddScoped<IPasswordHashService, PasswordHashService>();
            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();

            return services;
        }
   
    
    }

}
