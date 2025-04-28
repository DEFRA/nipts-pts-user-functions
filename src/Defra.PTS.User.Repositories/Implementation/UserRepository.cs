﻿using Entity = Defra.PTS.User.Entities;
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

        public async Task<Entity.User?> GetUser(string userEmailAddress)
        {
                return await UserContext?.User?.SingleOrDefaultAsync(a => a.Email == userEmailAddress)!;
        }

        [ExcludeFromCodeCoverage]
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
            var user = await UserContext?.User.Where(u => u.ContactId == contactId).FirstOrDefaultAsync()!;
            var address = await UserContext?.Address.Where(a => a.Id == user!.AddressId).FirstOrDefaultAsync()!;

            return new UserDetail
            {
                FullName = user?.FullName,
                Email = user?.Email,
                AddressId = address?.Id,
                Telephone = user?.Telephone,
                AddressLineOne = address?.AddressLineOne,
                AddressLineTwo = address?.AddressLineTwo,
                TownOrCity = address?.TownOrCity,
                County = address?.County,
                PostCode = address?.PostCode
            };
        }

    }
}
