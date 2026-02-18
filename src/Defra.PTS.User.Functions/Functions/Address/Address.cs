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

namespace Defra.PTS.User.Functions.Functions.Address
{
    public class Address
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<Address> _logger;
        private const string TagName = "CreateAddress";

        public Address(IAddressService addressService, ILogger<Address> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// Create Address
        /// </summary>
        /// <param name="req"></param>        
        /// <returns></returns>
        [Function("CreateAddress")]
        [OpenApiOperation(operationId: "CreateAddress", tags: TagName)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.Address), Description = "Create Address")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> CreateAddress(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createaddress")] HttpRequestData req)
        {
            var inputData = req.Body ?? throw new AddressFunctionException("Invalid Address input, is NUll or Empty");

            var addressModel = await _addressService.GetAddressModel(inputData);
            var addressId = await _addressService.CreateAddress(addressModel);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(addressId);
            return response;
        }
    }
}

