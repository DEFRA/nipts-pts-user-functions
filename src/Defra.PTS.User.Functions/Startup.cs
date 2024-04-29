using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Defra.PTS.User.ApiServices.Configuration;
using Defra.PTS.User.Functions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Defra.PTS.User.Functions.Startup))]
namespace Defra.PTS.User.Functions
{
    [ExcludeFromCodeCoverageAttribute]
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }


        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configurationBuilder = builder.GetContext().Configuration;
            var connection = string.Empty;
#if DEBUG

            connection = configurationBuilder["sql_db"];
#else
            connection = configurationBuilder.GetConnectionString("sql_db");
#endif

            builder.Services.AddDefraRepositoryServices(connection);
            builder.Services.AddDefraApiServices();
        }
    }
}
