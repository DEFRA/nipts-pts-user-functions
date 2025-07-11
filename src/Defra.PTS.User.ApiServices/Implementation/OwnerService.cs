using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Entity = Defra.PTS.User.Entities;
using Model = Defra.PTS.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Defra.PTS.User.Models.CustomException;
using Defra.PTS.User.Repositories.Implementation;
using Defra.PTS.User.Entities;
using Defra.PTS.User.Models.Enums;
using Defra.PTS.User.Models.Helper;

namespace Defra.PTS.User.ApiServices.Implementation
{
    public class OwnerService(
        IOwnerRepository ownerRepository,
        IRepository<Entity.Address> addressRepository) : IOwnerService
    {        
        private readonly IOwnerRepository _ownerRepository = ownerRepository;
        private readonly IRepository<Entity.Address> _addressRepository = addressRepository;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<Guid> CreateOwner(Model.Owner ownerModel)
        {            
            var addressDB = new Entity.Address()
            {
                AddressLineOne = ownerModel?.Address?.AddressLineOne,
                AddressLineTwo = ownerModel?.Address?.AddressLineTwo,
                TownOrCity = ownerModel?.Address?.TownOrCity,
                County = ownerModel?.Address?.County,
                CountryName = ownerModel?.Address?.CountryName,
                AddressType = AddressType.Owner.ToString(),
                PostCode = ownerModel?.Address?.PostCode,
                IsActive = true,
                CreatedBy = ownerModel?.Address?.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _addressRepository.Add(addressDB);
            await _addressRepository.SaveChanges();

            var ownerDB = new Entity.Owner()
            {
                 Email = ownerModel?.Email,
                 FullName = ownerModel?.FullName,
                 Telephone = ownerModel?.Telephone,
                 AddressId = addressDB.Id,                 
                 CreatedBy = ownerModel?.CreatedBy,
                 CreatedOn = DateTime.Now
            };
            await _ownerRepository.Add(ownerDB);
            await _ownerRepository.SaveChanges();           

            return ownerDB.Id;
        }

        public async Task<bool> DoesOwnerExists(string ownerEmail)
        {
            if (string.IsNullOrEmpty(ownerEmail))
            {
                throw new UserFunctionException("Invalid Owner Email Address");
            }

            return await _ownerRepository.DoesOwnerExists(ownerEmail);
        }

        public async Task<Owner?> GetOwnerByEmail(string ownerEmail)
        {
            if (string.IsNullOrEmpty(ownerEmail))
            {
                throw new UserFunctionException("Invalid Owner Email Address");
            }

            return await _ownerRepository.GetOwnerByEmail(ownerEmail);
        }

        public async Task<Model.Owner> GetOwnerModel(Stream userStream)
        {
            string owner = await new StreamReader(userStream).ReadToEndAsync();

            try
            {
                Model.Owner? ownerModel = JsonSerializer.Deserialize<Model.Owner>(owner, _jsonOptions);
                return ownerModel!;
            }

            catch
            {
                throw new UserFunctionException("Cannot create Owner as Owner Model Cannot be Deserialized");
            }
        }

        public async Task UpdateOwnerEmailsByOldEmail(string oldEmail, string newEmail)
        {
            if (string.IsNullOrEmpty(oldEmail) || string.IsNullOrEmpty(newEmail))
                return;

            var owners = await _ownerRepository.GetOwnersByEmailAsync(oldEmail);
            if (owners != null && owners.Count != 0)
            {
                foreach (var owner in owners)
                {
                    owner.Email = newEmail;
                    owner.UpdatedOn = DateTime.UtcNow;
                    _ownerRepository.Update(owner);
                }
                await _ownerRepository.SaveChanges();
            }
        }

    }
}
