using entity = Defra.PTS.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Interface
{
    public interface IUserRepository : IRepository<entity.User>
    {
        Task<bool> DoesUserExists(string userEmailAddress);
        Task<entity.User> GetUser(string userEmailAddress);
        Task<bool> PerformHealthCheckLogic();
        Task<UserDetail> GetUserDetail(Guid contactId);
    }
}
