using Microsoft.Extensions.DependencyInjection;
using Business.EndToEndService;

namespace Business
{

    public static class ServicesDependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPersonService, clsPersonService>();
            services.AddScoped<IUserService, clsUserService>();
            services.AddScoped<ITentantAuthService,TenantAuthService>();
            services.AddScoped<ITenantService, clsTenantService>();
            services.AddScoped<ITenantPlanService, clsTenantPlanService>();
            services.AddScoped<IEmailService, clsEmailService>();
            services.AddScoped<ITenantSessionService, clsTenantSessionService>();
            services.AddScoped<ITentantAuthService, TenantAuthService>();
           services.AddScoped<IEmailSettingsFactory, EnvironmentEmailSettingsFactory>();
            services.AddScoped<IJwtService, JwtService>();


            // business rules, domain services, validators, etc.

            return services;
        }
    }

}
