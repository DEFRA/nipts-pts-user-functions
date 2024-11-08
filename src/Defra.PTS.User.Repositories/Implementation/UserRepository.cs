using Entity = Defra.PTS.User.Entities;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;
using Defra.PTS.User.Entities;

namespace Defra.PTS.User.Repositories.Implementation
{
    [ExcludeFromCodeCoverageAttribute]
    public class UserRepository : Repository<Entity.User>, IUserRepository
    {

        private UserDbContext? UserContext
        {
            get
            {
                return _dbContext as UserDbContext;
            }
        }

        public UserRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> DoesUserExists(string userEmailAddress)
        {
           return await UserContext?.User.AnyAsync(a => a.Email == userEmailAddress!)!;
        }

        public async Task<Entity.User> GetUser(string userEmailAddress)
        {
            return await UserContext.User.SingleOrDefaultAsync(a => a.Email == userEmailAddress);
        }

        public async Task<bool> PerformHealthCheckLogic()
        {
            // Attempt to open a connection to the database
            await UserContext!.Database.OpenConnectionAsync();

            // Check if the connection is open
            if (UserContext.Database.GetDbConnection().State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<UserDetail> GetUserDetail(Guid contactId)
        {
            var query = await (
                from t1 in UserContext?.Address
                join t2 in UserContext?.User! on t1.Id equals t2.AddressId
                where t2.ContactId == contactId
                select new UserDetail
                {                    
                    FullName = t2.FullName,
                    Email = t2.Email,
                    AddressId = t2.AddressId,
                    Telephone = t2.Telephone,
                    AddressLineOne = t1.AddressLineOne,
                    AddressLineTwo = t1.AddressLineTwo,
                    TownOrCity = t1.TownOrCity,
                    County = t1.County,
                    PostCode = t1.PostCode,
                }
            ).FirstOrDefaultAsync();

            return query ?? new UserDetail();
        }

    }
}
