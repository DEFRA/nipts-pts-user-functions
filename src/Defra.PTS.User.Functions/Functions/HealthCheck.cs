using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Defra.PTS.User.Functions.Functions
{
    public class HealthCheck
    {
        private readonly IUserService _userService;
        private readonly ILogger<HealthCheck> _logger;

        public HealthCheck(IUserService userService, ILogger<HealthCheck> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Function("HealthCheck")]
        [OpenApiOperation(operationId: "HealthCheck", tags: new[] { "Health" }, Summary = "Health check endpoint", Description = "Returns the health status of the API")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The service is healthy")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.ServiceUnavailable, Description = "The service is unhealthy")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Perform health check logic here
            bool isHealthy = await _userService.PerformHealthCheckLogic();

            var response = isHealthy
                ? req.CreateResponse(HttpStatusCode.OK)
                : req.CreateResponse(HttpStatusCode.ServiceUnavailable);

            return response;
        }
    }
}

