using Microsoft.Extensions.DependencyInjection;
using Connection.models;

namespace Connection
{
    public static class ConnectionDependencyInjection
    {
        public static IServiceCollection AddConnectionDependencies(
            this IServiceCollection services)
        {
<<<<<<< HEAD
            services.AddScoped<IPersonRepository, clsPersonRepo>();
            services.AddScoped<IUserRepo, clsUserRepo>();
=======
            services.AddScoped<ICustomerRepo, clsCustomerRepository>();
>>>>>>> origin/main

            return services;
        }
    }
}
