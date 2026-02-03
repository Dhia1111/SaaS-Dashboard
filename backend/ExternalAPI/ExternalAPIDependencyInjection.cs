using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalAPI
{

    public static class ExternalAPIDependencyInjection
    {
        public static IServiceCollection AddExternalAPI(this IServiceCollection services)
        {
//            services.AddScoped<Ic, clsCustomer>();


            return services;
        }
    }

}
