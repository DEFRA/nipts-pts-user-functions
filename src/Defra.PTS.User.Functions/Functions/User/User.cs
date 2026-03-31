using System.Net;
using System.Text.Json;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Model = Defra.PTS.User.Models;
using Entity = Defra.PTS.User.Entities;

namespace Defra.PTS.User.Functions.Functions.User
{
    public class User(IUserService userService, IOwnerService ownerService, ILogger<User> logger)
    {
        private const string CreateUserTagName = "CreateUser";
      private const string UpdateUserTagName = "UpdateUser";
        private const string UpdateUserAddressTagName = "UpdateUserAddress";

      [Function("CreateUser")]
        public async Task<HttpResponseData> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createuser")] HttpRequestData? req)
        {
    if (req == null)
   {
      throw new UserFunctionException("Invalid user input, is NULL or Empty");
            }

       var inputData = req.Body ?? throw new UserFunctionException("Invalid user input, is NULL or Empty");
var userModel = await userService.GetUserModel(inputData) ?? throw new UserFunctionException("Failed to parse user model from input data");

        Guid userId;

if (HasValidContactId(userModel))
  {
                userId = await HandleContactIdBasedUser(userModel);
            }
else
       {
     userId = await HandleEmailBasedUser(userModel);
        }

var response = req.CreateResponse(HttpStatusCode.OK);
    await response.WriteAsJsonAsync(userId);
            return response;
        }

        private static bool HasValidContactId(Model.User userModel)
        {
          return userModel.ContactId.HasValue && userModel.ContactId.Value != Guid.Empty;
        }

        private async Task<Guid> HandleContactIdBasedUser(Model.User userModel)
        {
        var existingUser = await userService.GetUserByContactId(userModel.ContactId!.Value);

  if (existingUser == null)
            {
           return await HandleEmailBasedUser(userModel);
    }

        await ProcessEmailUpdate(existingUser, userModel);
    await UpdateSignInTime(userModel.Email);

 return existingUser.Id;
        }

    private async Task ProcessEmailUpdate(Entity.User existingUser, Model.User userModel)
    {
         var existingUserEmail = existingUser.Email;
            var newEmail = userModel.Email;
   if (!IsEmailChanged(existingUserEmail, newEmail))
            {
    return;
        }

   logger.LogInformation("Email changed for ContactId {ContactId} from {OldEmail} to {NewEmail}",
            userModel.ContactId, existingUserEmail, newEmail);

            try
       {
              await userService.UpdateUserEmail(existingUserEmail, newEmail);
           await ownerService.UpdateOwnerEmailsByOldEmail(existingUserEmail, newEmail);
      logger.LogInformation("Successfully updated user and owner emails for ContactId {ContactId}", userModel.ContactId);
     }
catch (Exception ex)
            {
          logger.LogError(ex, "Failed to update emails for ContactId {ContactId}: {ErrorMessage}",
        userModel.ContactId, ex.Message);
        }
        }

        private static bool IsEmailChanged(string existingEmail, string newEmail)
        {
         return !string.IsNullOrEmpty(existingEmail) &&
     !string.IsNullOrEmpty(newEmail) &&
    !string.Equals(existingEmail, newEmail, StringComparison.OrdinalIgnoreCase);
        }

   private async Task UpdateSignInTime(string email)
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
        logger.LogWarning(ex, "Failed to update sign-in time for user {Email}", email);
    }
        }

        private async Task<Guid> HandleEmailBasedUser(Model.User userModel)
        {
            if (string.IsNullOrEmpty(userModel.Email))
  {
            throw new UserFunctionException("User model must have either ContactId or Email");
            }

            bool userExists = await userService.DoesUserExists(userModel.Email);

          if (!userExists)
            {
      logger.LogInformation("Creating new user for email {Email}", userModel.Email);
Guid userId = await userService.CreateUser(userModel);
      return userId;
    }

        await UpdateSignInTime(userModel.Email);
            var existingUserId = await userService.GetUserIdAsync(userModel.Email);
            return existingUserId;
        }

     /// <summary>
      /// Update User
        /// </summary>
  /// <param name="req"></param>
        /// <returns></returns>
   [Function("UpdateUser")]
        public async Task<HttpResponseData> UpdateUser(
     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updateuser")] HttpRequestData? req)
     {
            if (req == null)
      {
          throw new UserFunctionException("Invalid user input, is NUll or Empty");
            }

            var inputData = req.Body ?? throw new UserFunctionException("Invalid user input, is NUll or Empty");
 var userEmailModel = await userService.GetUserEmailModel(inputData);

   var response = req.CreateResponse(HttpStatusCode.OK);

            if (await userService.DoesUserExists(userEmailModel.Email))
   {
 var userId = await userService.UpdateUser(userEmailModel.Email, userEmailModel.Type);
     logger.LogInformation("User updated with ID: {0}", userId);
        await response.WriteAsJsonAsync(userId);
            }
            else
        {
      await response.WriteAsJsonAsync("Cannot update new User as user does not exists");
            }

     return response;
        }

      /// <summary>
     /// Update User Address
     /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("UpdateUserAddress")]
        public async Task<HttpResponseData> UpdateUserAddress(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updateuseraddress")] HttpRequestData? req)
        {
      if (req == null)
          {
     throw new UserFunctionException("Invalid user input, is NUll or Empty");
            }

var inputData = req.Body ?? throw new UserFunctionException("Invalid user input, is NUll or Empty");
     var userEmailModel = await userService.GetUserEmailModel(inputData);

            var response = req.CreateResponse(HttpStatusCode.OK);

            if (await userService.DoesUserExists(userEmailModel.Email))
 {
        var userId = await userService.UpdateUser(userEmailModel.Email, userEmailModel.AddressId);
        logger.LogInformation("User updated with ID: {0}", userId);
     await response.WriteAsJsonAsync(userId);
 }
            else
            {
      await response.WriteAsJsonAsync("Cannot update new User as user does not exists");
            }

      return response;
  }
    }
}

