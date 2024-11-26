using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Defra.PTS.User.ApiServices.Implementation;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models;
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

namespace Defra.PTS.User.Functions.Functions.User
{
    public class User
    {
        private readonly IUserService _userService;
        private const string CreateUserTagName = "CreateUser";
        private const string UpdateUserTagName = "UpdateUser";
        private const string UpdateUserAddressTagName = "UpdateUserAddress";


        public User(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { CreateUserTagName })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.User), Description = "Create User")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createuser")] HttpRequest req,
            ILogger log)
        {
            var inputData = req?.Body;
            if (inputData == null)
            {
                throw new UserFunctionException("Invalid user input, is NUll or Empty");
            }

            var userModel = await _userService.GetUserModel(inputData);
            if (!await _userService.DoesUserExists(userModel.Email))
            {
                Guid userId = await _userService.CreateUser(userModel);
                return new OkObjectResult(userId);
            }
            else
            {
                if (!string.IsNullOrEmpty(userModel.Email))
                {
                    _ = await _userService.UpdateUser(userModel.Email, "signin");
                }
                    
                var userId = await _userService.GetUserIdAsync(userModel.Email);
                    
                return new OkObjectResult(userId);                    
            }       
        }


        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("UpdateUser")]
        [OpenApiOperation(operationId: "UpdateUser", tags: new[] { UpdateUserTagName })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.UserEmail), Description = "UpdateUser")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updateuser")] HttpRequest req,
            ILogger log)
        {

            var inputData = req?.Body;
            if (inputData == null)
            {
                throw new UserFunctionException("Invalid user input, is NUll or Empty");
            }

            var userEmailModel = await _userService.GetUserEmailModel(inputData);
            if (await _userService.DoesUserExists(userEmailModel.Email))
            {
                var userId = await _userService.UpdateUser(userEmailModel.Email, userEmailModel.Type);
                log.LogInformation("User updated with ID: {0}", userId);
                return new OkObjectResult(userId);
            }

            return new OkObjectResult("Cannot update new User as user does not exists");
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("UpdateUserAddress")]
        [OpenApiOperation(operationId: "UpdateUserAddress", tags: new[] { UpdateUserAddressTagName })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.UserEmail), Description = "UpdateUserAddress")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> UpdateUserAddress(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updateuseraddress")] HttpRequest req,
            ILogger log)
        {
            var inputData = req?.Body;
            if (inputData == null)
            {
                throw new UserFunctionException("Invalid user input, is NUll or Empty");
            }

            var userEmailModel = await _userService.GetUserEmailModel(inputData);
            if (await _userService.DoesUserExists(userEmailModel.Email))
            {
                var userId = await _userService.UpdateUser(userEmailModel.Email, userEmailModel.AddressId);
                log.LogInformation("User updated with ID: ", userId);
                return new OkObjectResult(userId);
            }

            return new OkObjectResult("Cannot update new User as user does not exists");
        }
    }
}

