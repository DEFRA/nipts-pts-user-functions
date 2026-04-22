using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
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
        [OpenApiOperation(operationId: "CreateOwner", tags: new[] { "Owner" }, Summary = "Create a new owner", Description = "Creates a new owner/traveller in the system")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.Owner), Required = true, Description = "Owner data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Guid), Description = "Owner created successfully")]
        public async Task<HttpResponseData> CreateTraveller(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createowner")] HttpRequestData? req)
        {
            if (req == null)
            {
                throw new UserFunctionException("Invalid Owner input, is NUll or Empty");
            }

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

