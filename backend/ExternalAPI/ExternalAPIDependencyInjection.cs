
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
           
            services.AddScoped<IEmailExternalService, clsExternalEmailService>();
            return services;
        }
   
    
    }

}
