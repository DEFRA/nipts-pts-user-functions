using System.Net;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.Functions.Functions.Address
{
    public class Address
    {
        private readonly IAddressService _addressService;

        public Address(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Create Address
        /// </summary>
        /// <param name="req"></param>        
        /// <returns></returns>
        [Function("CreateAddress")]
        [OpenApiOperation(operationId: "CreateAddress", tags: new[] { "Address" }, Summary = "Create a new address", Description = "Creates a new address in the system")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.Address), Required = true, Description = "Address data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Guid), Description = "Address created successfully")]
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

