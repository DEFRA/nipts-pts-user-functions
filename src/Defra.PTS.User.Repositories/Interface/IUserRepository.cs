using Entity = Defra.PTS.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Interface
{
    public interface IUserRepository : IRepository<Entity.User>
    {
        Task<bool> DoesUserExists(string userEmailAddress);
        Task<Entity.User> GetUser(string userEmailAddress);
        Task<bool> PerformHealthCheckLogic();
        Task<UserDetail> GetUserDetail(Guid contactId);
    }
}
