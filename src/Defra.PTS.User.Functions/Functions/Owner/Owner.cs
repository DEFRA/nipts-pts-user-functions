using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Defra.PTS.User.ApiServices.Implementation;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using model = Defra.PTS.User.Models;

namespace Defra.PTS.User.Functions.Functions.Owner
{
    public class Owner
    {
        private readonly IOwnerService _ownerService;

        public Owner(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Create Traveller
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateOwner")]
        [OpenApiOperation(operationId: "CreateOwner", tags: new[] { "CreateOwner" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(model.Owner), Description = "Create Traveller")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateTraveller(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createowner")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var inputData = req?.Body;
                if (inputData == null)
                {
                    throw new UserFunctionException("Invalid Owner input, is NUll or Empty");
                }

                var ownerModel = await _ownerService.GetOwnerModel(inputData);

                if (!await _ownerService.DoesOwnerExists(ownerModel.Email))
                {
                    Guid travellerId = await _ownerService.CreateOwner(ownerModel);
                    return new OkObjectResult(travellerId);
                }

                var ownerDbEntry = await _ownerService.GetOwnerByEmail(ownerModel.Email);

                return new OkObjectResult(ownerDbEntry.Id);
            }
            catch (Exception ex)
            {
                log.LogError("Error Stack: " + ex.StackTrace);
                log.LogError("Exception Message: " + ex.Message);
                throw;
            }            
        }
    }
}

