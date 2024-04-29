using Defra.PTS.User.ApiServices.Implementation;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.User.Functions.Configuration
{
    [ExcludeFromCodeCoverageAttribute]
    public static class ConfigureApi
    {
        public static IServiceCollection AddDefraApiServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IAddressService, AddressService>();
            return services;
        }
    }
}
