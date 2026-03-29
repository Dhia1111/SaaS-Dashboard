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

            // business rules, domain services, validators, etc.

            return services;
        }
    }

}
