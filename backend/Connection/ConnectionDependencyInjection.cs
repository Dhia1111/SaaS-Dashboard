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
            services.AddScoped<ITenantPlanRepo, clsTenantPlanRepo>();
            services.AddScoped<IsessionRepo, clsSessionRepo>();
            return services;
        }
    }
}
