using Microsoft.Extensions.DependencyInjection;


namespace ExternalAPI
{

    public static class ExternalAPIDependencyInjection
    {
        public static IServiceCollection AddExternalAPIDependencies(this IServiceCollection services)
        {
//            services.AddScoped<Ic, clsCustomer>();


            return services;
        }
    }

}
