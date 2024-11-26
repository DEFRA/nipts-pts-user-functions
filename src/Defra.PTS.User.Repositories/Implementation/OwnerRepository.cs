using Entity = Defra.PTS.User.Entities;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.PTS.User.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.User.Repositories.Implementation
{
    [ExcludeFromCodeCoverageAttribute]
    public class OwnerRepository : Repository<Entity.Owner>, IOwnerRepository
    {

        private UserDbContext? userContext
        {
            get
            {
                return _dbContext as UserDbContext;
            }
        }

        public OwnerRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Owner?> GetOwnerByEmail(string ownerEmailAddress)
        {
            return await userContext?.Owner?.FirstOrDefaultAsync(a => a.Email == ownerEmailAddress)!;
        }

        public async Task<bool> DoesOwnerExists(string ownerEmailAddress)
        {
           return await userContext?.Owner?.AnyAsync(a => a.Email == ownerEmailAddress)!;
        }
    }
}
