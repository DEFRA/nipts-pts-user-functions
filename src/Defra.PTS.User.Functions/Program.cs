using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Defra.PTS.User.ApiServices.Configuration;
using Defra.PTS.User.Functions.Configuration;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureAppConfiguration((context, config) =>
    {
        config
 .SetBasePath(context.HostingEnvironment.ContentRootPath)
      .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
     services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var configuration = context.Configuration;
        var connection = string.Empty;

#if DEBUG
     connection = configuration["sql_db"];
#else
        connection = configuration.GetConnectionString("sql_db");
#endif

     services.AddDefraRepositoryServices(connection);
        services.AddDefraApiServices();
    })
    .Build();

host.Run();
