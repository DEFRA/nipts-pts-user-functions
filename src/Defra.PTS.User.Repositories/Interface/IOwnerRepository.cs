using Entity = Defra.PTS.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Interface
{
    public interface IOwnerRepository : IRepository<Entity.Owner>
    {
        Task<bool> DoesOwnerExists(string ownerEmailAddress);
        Task<Owner> GetOwnerByEmail(string ownerEmailAddress);
    }
}
