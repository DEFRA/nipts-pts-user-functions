using Entity = Defra.PTS.User.Entities;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Implementation
{
    public class OwnerRepository(DbContext dbContext) : Repository<Entity.Owner>(dbContext), IOwnerRepository
    {

        private UserDbContext? UserContext
        {
            get
            {
                return _dbContext as UserDbContext;
            }
        }

        public async Task<Owner?> GetOwnerByEmail(string ownerEmailAddress)
        {
            return await UserContext?.Owner?.FirstOrDefaultAsync(a => a.Email == ownerEmailAddress)!;
        }

        public async Task<bool> DoesOwnerExists(string ownerEmailAddress)
        {
           return await UserContext?.Owner?.AnyAsync(a => a.Email == ownerEmailAddress)!;
        }

        public async Task<List<Owner>> GetOwnersByEmailAsync(string email)
        {
            if (UserContext?.Owner == null)
                return [];

            return await UserContext.Owner
                .Where(o => o.Email == email)
                .ToListAsync();
        }

    }
}
