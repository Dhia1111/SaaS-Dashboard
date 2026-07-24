using Microsoft.Extensions.DependencyInjection;
using Connection.models;

namespace Connection
{
    public static class ConnectionDependencyInjection
    {
        public static IServiceCollection AddConnectionDependencies(
            this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, clsPersonRepo>();
            services.AddScoped<IUserRepo, clsUserRepo>();
            services.AddScoped<ITenantRepo, clsTenantRepo>();
            services.AddScoped<IEmailRepository, clsEmailRepository>();
             services.AddScoped<ITenantsessionRepo, clsSessionRepo>();
            services.AddScoped<IUserSessionRepo, clsUserSessionRepo>();
            services.AddScoped<ITenantPermissionRepository, TenantPermissionRepository>();
            services.AddScoped<ITenantPlanRepository, TenantPlanRepository>();
            services.AddScoped<ITenantPlanBenifestRepository, TenantPlanBenifestRepository>();
            services.AddScoped<ITenantPlanPermissionRepository, TenantPlanPermissionRepository>();
            services.AddScoped<ITenantPricingOptionRepository, TenantPricingOptionRepository>();
            services.AddScoped<ITenantPricingCycleRepository, TenantPricingCycleRepository>();
            services.AddScoped<IEmployeeRepo, clsEmployeeRepo>();
            services.AddScoped<IPlatformAdmineRepo, clsPlatformAdmineRepo>();
       
            services.AddScoped<IPaymentRepo, clsPaymentRepo>();
            services.AddScoped<IPlatformSubscriptionRepo, clsPlatformSubscriptionRepo>();
        
            services.AddScoped<IDiscoveryPlatformRepo, clsDiscoveryPlatformRepo>();

            services.AddScoped<ITenantFreePlanRepo, clsTenantFreePlanRepo>();

            services.AddScoped<IClientSubscriptionRepo, clsClientSubscriptionRepo>();

            services.AddScoped<ITenantAccessStateService, clsTenantAccessStateService>();




            return services;
        }
    }
}
