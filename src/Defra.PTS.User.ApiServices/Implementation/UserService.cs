using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Repositories.Interface;
using entity = Defra.PTS.User.Entities;
using model = Defra.PTS.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Defra.PTS.User.Models.Enums;
using Defra.PTS.User.Models.CustomException;
using Defra.PTS.User.Repositories;
using static System.Net.Mime.MediaTypeNames;
using Defra.PTS.User.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.User.ApiServices.Implementation
{    
    public class UserService : IUserService
    {        
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {            
            _userRepository = userRepository;            
        }

        [ExcludeFromCodeCoverage]
        public async Task<Guid> CreateUser(model.User userModel)
        {
            var userDB = new entity.User()
            {
                Email = userModel.Email,
                FullName = userModel.FullName,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Role = userModel.Role,
                Telephone = userModel.Telephone,
                ContactId = userModel.ContactId,
                Uniquereference = userModel.Uniquereference,
                SignInDateTime = userModel.SignInDateTime,
                SignOutDateTime = userModel.SignInDateTime,
                CreatedBy = userModel.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _userRepository.Add(userDB);
            await _userRepository.SaveChanges();

            return userDB.Id;
        }

        [ExcludeFromCodeCoverage]
        public async Task<Guid> GetUserIdAsync(string userEmail)
        {
            var user = await _userRepository.GetUser(userEmail);
            return user.Id;
        }

        public async Task<Guid> UpdateUser(string userEmail, string type)
        {

            var userDB = _userRepository.GetUser(userEmail).Result;
            if (type == "signin")
                userDB.SignInDateTime = DateTime.Now;
            else
                userDB.SignOutDateTime = DateTime.Now;

            _userRepository.Update(userDB);
            await _userRepository.SaveChanges();

            return userDB.Id;

        }
        [ExcludeFromCodeCoverage]
        public async Task<Guid> UpdateUser(string userEmail, Guid? addressId)
        {

            var userDB = _userRepository.GetUser(userEmail).Result;
            userDB.AddressId = addressId;
            userDB.UpdatedOn = DateTime.Now;

            _userRepository.Update(userDB);
            await _userRepository.SaveChanges();

            return userDB.Id;

        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> DoesUserExists(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new UserFunctionException("Invalid User Email Address");
            }

            return await _userRepository.DoesUserExists(userEmail);
        }

        public async Task<model.User> GetUserModel(Stream userStream)
        {
            try
            {
                string user = await new StreamReader(userStream).ReadToEndAsync();
                model.User? userModel = JsonSerializer.Deserialize<model.User>(user, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return userModel!;
            }
            catch
            {
                throw new UserFunctionException("Cannot create User as User Model Cannot be Deserialized");
            }
        }

        public async Task<model.UserEmail> GetUserEmailModel(Stream userStream)
        {
            try
            {
                string userEmail = await new StreamReader(userStream).ReadToEndAsync();
                model.UserEmail? userEmailModel = JsonSerializer.Deserialize<model.UserEmail>(userEmail, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
   
                return userEmailModel!;
            }
            catch
            {
                throw new UserFunctionException("Cannot create User as UserEmail Model Cannot be Deserialized");
            }        
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> PerformHealthCheckLogic()
        {
            return await _userRepository.PerformHealthCheckLogic();
        }

        [ExcludeFromCodeCoverage]
        public async Task<UserDetail> GetUserDetail(Guid contactId)
        {
            return await _userRepository.GetUserDetail(contactId);
        }

    }
}
