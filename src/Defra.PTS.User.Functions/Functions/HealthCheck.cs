using System.IO;
using System.Net;
using System.Threading.Tasks;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Defra.PTS.User.Functions.Functions
{
    public class HealthCheck
    {
        private readonly IUserService _userService;
        public HealthCheck(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName("HealthCheck")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req
            , ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            // Perform health check logic here
            bool isHealthy = await _userService.PerformHealthCheckLogic();

            if (isHealthy)
            {
                return new OkResult();
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}

