using Defra.PTS.User.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories
{
    [ExcludeFromCodeCoverageAttribute]
    public class UserDbContext :DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
                
        }

        public DbSet<entity.User> User { get; set; }
        public DbSet<entity.Owner> Owner { get; set; }
        public DbSet<entity.Address> Address { get; set; }
        
    }
}