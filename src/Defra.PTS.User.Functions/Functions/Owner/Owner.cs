using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.Functions.Functions.Owner
{
    public class Owner
    {
        private readonly IOwnerService _ownerService;
        private readonly ILogger<Owner> _logger;
        private const string TagName = "CreateOwner";

        public Owner(IOwnerService ownerService, ILogger<Owner> logger)
        {
            _ownerService = ownerService;
            _logger = logger;
        }

        /// <summary>
        /// Create Traveller
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("CreateOwner")]
        [OpenApiOperation(operationId: "CreateOwner", tags: TagName)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.Owner), Description = "Create Traveller")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> CreateTraveller(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createowner")] HttpRequestData req)
        {
            var inputData = req.Body ?? throw new UserFunctionException("Invalid Owner input, is NUll or Empty");

            var ownerModel = await _ownerService.GetOwnerModel(inputData);

            Guid ownerId;

            if (!await _ownerService.DoesOwnerExists(ownerModel.Email))
            {
                ownerId = await _ownerService.CreateOwner(ownerModel);
            }
            else
            {
                var ownerDbEntry = await _ownerService.GetOwnerByEmail(ownerModel.Email);
                ownerId = ownerDbEntry.Id;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(ownerId);
            return response;
        }
    }
}

