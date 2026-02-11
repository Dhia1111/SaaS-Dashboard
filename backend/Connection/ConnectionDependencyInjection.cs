using Connection.models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{

    public static class ConnectionDependencyInjection
    {
        public static IServiceCollection AddConnectionDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, clsPersonRepo>();
            services.AddScoped<IUserRepo, clsUserRepo>();

            return services;
        }
    }

}
