using Microsoft.Extensions.DependencyInjection;
using Connection.models;

namespace Connection
{
    public static class ConnectionDependencyInjection
    {
        public static IServiceCollection AddConnectionDependencies(
            this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepo, clsCustomerRepository>();

            return services;
        }
    }
}
