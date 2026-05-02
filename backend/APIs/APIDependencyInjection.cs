
using APIs.AssetHandler;
using APIs.BackGroundJobs;
using APIs.Hashing;
using APIs.TokenHandler;
using Business;
using Connection.models;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Quartz.Serialization.Json;



namespace ExternalAPI
{

    public static class APIDependencyInjection
    {
        public static IServiceCollection AddAPIDependencies(this IServiceCollection services)
        {
            services.AddScoped<IEmailTemplateHandler, EmailTemplateHandler>();
            services.AddScoped<IPasswordHashService, PasswordHashService>();
            services.AddSingleton<IGenralHashService, GenralHashService>();
            services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddTransient<EmailBackgroundJob>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantIdProvider, APIs.ConfigClasses.TenantIdProvider>();
            return services;
        }
   
    
    }

}
