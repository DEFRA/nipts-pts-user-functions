﻿using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Repositories.Interface;
using Entity = Defra.PTS.User.Entities;
using Model = Defra.PTS.User.Models;
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

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<Guid> CreateUser(Model.User userModel)
        {
            var userDB = new Entity.User()
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

        public async Task<Guid> GetUserIdAsync(string userEmail)
        {
            var user = await _userRepository.GetUser(userEmail);
            return user!.Id;
        }

        public async Task<Guid> UpdateUser(string userEmail, string type)
        {

            var userDB = _userRepository.GetUser(userEmail).Result;
            if (type == "signin")
                userDB!.SignInDateTime = DateTime.Now;
            else
                userDB!.SignOutDateTime = DateTime.Now;

            _userRepository.Update(userDB);
            await _userRepository.SaveChanges();

            return userDB.Id;

        }

        public async Task<Guid> UpdateUser(string userEmail, Guid? addressId)
        {

            var userDB = _userRepository.GetUser(userEmail).Result;
            userDB!.AddressId = addressId;
            userDB.UpdatedOn = DateTime.Now;

            _userRepository.Update(userDB);
            await _userRepository.SaveChanges();

            return userDB.Id;

        }

        public async Task<bool> DoesUserExists(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new UserFunctionException("Invalid User Email Address");
            }

            return await _userRepository.DoesUserExists(userEmail);
        }

        public async Task<Model.User> GetUserModel(Stream userStream)
        {
            try
            {
                string user = await new StreamReader(userStream).ReadToEndAsync();
                Model.User? userModel = JsonSerializer.Deserialize<Model.User>(user, _jsonOptions);

                return userModel!;
            }
            catch
            {
                throw new UserFunctionException("Cannot create User as User Model Cannot be Deserialized");
            }
        }

        public async Task<Model.UserEmail> GetUserEmailModel(Stream userStream)
        {
            try
            {
                string userEmail = await new StreamReader(userStream).ReadToEndAsync();
                Model.UserEmail? userEmailModel = JsonSerializer.Deserialize<Model.UserEmail>(userEmail, _jsonOptions);
   
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

        public async Task<UserDetail> GetUserDetail(Guid contactId)
        {
            return await _userRepository.GetUserDetail(contactId);
        }

    }
}
