using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.Functions.Functions.Address
{
    public class Address
    {
        private readonly IAddressService _addressService;

        private const string TagName = "CreateAddress";

        public Address(
              IAddressService addressService)
        {
            _addressService = addressService;            
        }

        /// <summary>
        /// Create Address
        /// </summary>
        /// <param name="req"></param>        
        /// <returns></returns>
        [FunctionName("CreateAddress")]
        [OpenApiOperation(operationId: "CreateAddress", tags: new[] { TagName })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.Address), Description = "Create Address")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateAddress(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createaddress")] HttpRequest req)
        {

            var inputData = req?.Body;
            if (inputData == null)
            {
                throw new AddressFunctionException("Invalid Address input, is NUll or Empty");
            }

            var addressModel = await _addressService.GetAddressModel(inputData);
            var addressId = await _addressService.CreateAddress(addressModel);

            return new OkObjectResult(addressId);
        }
    }
}

