using Entity = Defra.PTS.User.Entities;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Interface
{    
    public interface IOwnerRepository : IRepository<Entity.Owner>
    {
        Task<bool> DoesOwnerExists(string ownerEmailAddress);
        Task<Owner?> GetOwnerByEmail(string ownerEmailAddress);
        Task<List<Owner>> GetOwnersByEmailAsync(string email);
    }
}
