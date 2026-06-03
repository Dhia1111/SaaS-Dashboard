using Microsoft.Extensions.DependencyInjection;
using Business.EndToEndService;
using Business.Config;

namespace Business
{

    public static class ServicesDependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPersonService, clsPersonService>();
            services.AddScoped<IUserService, clsUserService>();
            services.AddScoped<ITenantAuthService,TenantAuthService>();
            services.AddScoped<ITenantService, clsTenantService>();
            services.AddScoped<ITenantPlanServices, clsTenantPlanServices>();
            services.AddScoped<IEmailService, clsEmailService>();
            services.AddScoped<ITenantSessionService, clsTenantSessionService>();
            services.AddScoped<ITenantAuthService, TenantAuthService>();
            services.AddScoped<IEmailSettingsFactory, EnvironmentEmailSettingsFactory>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IUserSessionService, clsUserSessionService>();

            services.AddScoped<ITenantPermissionServices, clsTenantPermissionServices>();
            services.AddScoped<ITenantPlanBenifestServices, clsTenantPlanBenifestServices>();
            services.AddScoped<ITenantPlanPermissionServices, clsTenantPlanPermissionServices>();
            services.AddScoped<ITenantPricingOptionServices, clsTenantPricingOptionServices>();
            //
            services.AddScoped<IRefreshTokenService, TenantAuthService>();
            services.AddScoped<IRefreshTokenService, UserAuthService>();
            //
            services.AddSingleton<INamingCookies, NamingCookies>();





            // business rules, domain services, validators, etc.

            return services;
        }
    }

}
