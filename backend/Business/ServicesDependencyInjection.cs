using Microsoft.Extensions.DependencyInjection;


namespace Business
{

    public static class ServicesDependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
   services.AddScoped<ICustomerService, clsCustomer>();

            // business rules, domain services, validators, etc.

            return services;
        }
    }

}
