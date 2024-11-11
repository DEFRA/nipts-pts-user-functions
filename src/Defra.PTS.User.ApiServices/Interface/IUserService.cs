using Defra.PTS.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.ApiServices.Interface
{
    public interface IUserService
    {
        Task<Model.User> GetUserModel(Stream userStream);
        Task<bool> DoesUserExists(string userEmail);
        Task<Guid> CreateUser(Model.User userModel);
        Task<Guid> GetUserIdAsync(string userEmail);
        Task<Guid> UpdateUser(string userEmail, string type);
        Task<Guid> UpdateUser(string userEmail, Guid? addressId);
        Task<Model.UserEmail> GetUserEmailModel(Stream userStream);
        Task<bool> PerformHealthCheckLogic();
        Task<UserDetail> GetUserDetail(Guid contactId);
    }
}   
