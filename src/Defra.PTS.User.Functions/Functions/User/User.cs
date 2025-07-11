using System;
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
using Model = Defra.PTS.User.Models;
using Entity = Defra.PTS.User.Entities;

namespace Defra.PTS.User.Functions.Functions.User
{
    public class User(IUserService userService, IOwnerService ownerService)
    {
        private const string CreateUserTagName = "CreateUser";
        private const string UpdateUserTagName = "UpdateUser";
        private const string UpdateUserAddressTagName = "UpdateUserAddress";

        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { CreateUserTagName })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Model.User), Description = "Create User")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createuser")] HttpRequest req,
    ILogger log)
        {
            var inputData = (req?.Body) ?? throw new UserFunctionException("Invalid user input, is NULL or Empty");
            var userModel = await userService.GetUserModel(inputData) ?? throw new UserFunctionException("Failed to parse user model from input data");

            if (HasValidContactId(userModel))
            {
                return await HandleContactIdBasedUser(userModel, log);
            }

            return await HandleEmailBasedUser(userModel, log);
        }

        private static bool HasValidContactId(Model.User userModel)
        {
            return userModel.ContactId.HasValue && userModel.ContactId.Value != Guid.Empty;
        }

        private async Task<IActionResult> HandleContactIdBasedUser(Model.User userModel, ILogger log)
        {
            var existingUser = await userService.GetUserByContactId(userModel.ContactId!.Value);

            if (existingUser == null)
            {
                return await HandleEmailBasedUser(userModel, log);
            }

            await ProcessEmailUpdate(existingUser, userModel, log);
            await UpdateSignInTime(userModel.Email, log);

            return new OkObjectResult(existingUser.Id);
        }

        private async Task ProcessEmailUpdate(Entity.User existingUser, Model.User userModel, ILogger log)
        {
            if (!IsEmailChanged(existingUser.Email, userModel.Email))
            {
                return;
            }

            log.LogInformation("Email changed for ContactId {ContactId} from {OldEmail} to {NewEmail}",
                userModel.ContactId, existingUser.Email, userModel.Email);

            try
            {
                await userService.UpdateUserEmail(existingUser.Email!, userModel.Email!);
                await ownerService.UpdateOwnerEmailsByOldEmail(existingUser.Email!, userModel.Email!);
                log.LogInformation("Successfully updated user and owner emails for ContactId {ContactId}", userModel.ContactId);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to update emails for ContactId {ContactId}: {ErrorMessage}",
                    userModel.ContactId, ex.Message);
            }
        }

        private static bool IsEmailChanged(string? existingEmail, string? newEmail)
        {
            return !string.IsNullOrEmpty(existingEmail) &&
                   !string.IsNullOrEmpty(newEmail) &&
                   !string.Equals(existingEmail, newEmail, StringComparison.OrdinalIgnoreCase);
        }

        private async Task UpdateSignInTime(string? email, ILogger log)
        {
            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            try
            {
                await userService.UpdateUser(email, "signin");
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Failed to update sign-in time for user {Email}", email);
            }
        }

        private async Task<IActionResult> HandleEmailBasedUser(Model.User userModel, ILogger log)
        {
            if (string.IsNullOrEmpty(userModel.Email))
            {
                throw new UserFunctionException("User model must have either ContactId or Email");
            }

            bool userExists = await userService.DoesUserExists(userModel.Email);

            if (!userExists)
            {
                log.LogInformation("Creating new user for email {Email}", userModel.Email);
                Guid userId = await userService.CreateUser(userModel);
                return new OkObjectResult(userId);
            }

            await UpdateSignInTime(userModel.Email, log);
            var existingUserId = await userService.GetUserIdAsync(userModel.Email);
            return new OkObjectResult(existingUserId);
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

            var inputData = (req?.Body) ?? throw new UserFunctionException("Invalid user input, is NUll or Empty");
            var userEmailModel = await userService.GetUserEmailModel(inputData);
            if (await userService.DoesUserExists(userEmailModel.Email))
            {
                var userId = await userService.UpdateUser(userEmailModel.Email, userEmailModel.Type);
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
            var inputData = (req?.Body) ?? throw new UserFunctionException("Invalid user input, is NUll or Empty");
            var userEmailModel = await userService.GetUserEmailModel(inputData);
            if (await userService.DoesUserExists(userEmailModel.Email))
            {
                var userId = await userService.UpdateUser(userEmailModel.Email, userEmailModel.AddressId);
                log.LogInformation("User updated with ID: ", userId);
                return new OkObjectResult(userId);
            }

            return new OkObjectResult("Cannot update new User as user does not exists");
        }
    }
}

