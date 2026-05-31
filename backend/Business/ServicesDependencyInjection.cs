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
            services.AddScoped<ITenantPlanServices, clsTenantPlanServices>();
            services.AddScoped<IEmailService, clsEmailService>();
            services.AddScoped<ITenantSessionService, clsTenantSessionService>();
            services.AddScoped<ITentantAuthService, TenantAuthService>();
            services.AddScoped<IEmailSettingsFactory, EnvironmentEmailSettingsFactory>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IUserSessionService, clsUserSessionService>();

            services.AddScoped<ITenantPermissionServices, clsTenantPermissionServices>();
            services.AddScoped<ITenantPlanBenifestServices, clsTenantPlanBenifestServices>();
            services.AddScoped<ITenantPlanPermissionServices, clsTenantPlanPermissionServices>();
            services.AddScoped<ITenantPricingOptionServices, clsTenantPricingOptionServices>();





            // business rules, domain services, validators, etc.

            return services;
        }
    }

}
