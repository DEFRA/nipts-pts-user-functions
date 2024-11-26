using Defra.PTS.User.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Entity = Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories
{
    [ExcludeFromCodeCoverageAttribute]
    public class UserDbContext :DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
                
        }

        public DbSet<Entity.User> User { get; set; }
        public DbSet<Entity.Owner> Owner { get; set; }
        public DbSet<Entity.Address> Address { get; set; }
        
    }
}