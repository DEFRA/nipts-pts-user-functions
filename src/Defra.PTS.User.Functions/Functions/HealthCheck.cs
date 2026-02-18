using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Defra.PTS.User.Functions.Functions
{
    public class HealthCheck
    {
        private readonly IUserService _userService;
        private readonly ILogger<HealthCheck> _logger;
        private const string TagName = "name";

        public HealthCheck(IUserService userService, ILogger<HealthCheck> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Function("HealthCheck")]
        [OpenApiOperation(operationId: "Run", tags: TagName)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
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

