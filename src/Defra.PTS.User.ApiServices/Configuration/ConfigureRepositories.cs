using Defra.PTS.User.Repositories;
using Defra.PTS.User.Repositories.Implementation;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.User.ApiServices.Configuration
{
    [ExcludeFromCodeCoverageAttribute]
    public static class ConfigureRepositories
    {        public static IServiceCollection AddDefraRepositoryServices(this IServiceCollection services, string conn)
        {
            services.AddDbContext<UserDbContext>((context) =>
            {
                context.UseSqlServer(conn);
            });
            services.AddScoped<DbContext, UserDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}
