using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.Extensions.Logging;
using entity = Defra.PTS.User.Entities;
using model = Defra.PTS.User.Models;
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
    public class OwnerService : IOwnerService
    {        
        private readonly IOwnerRepository _ownerRepository;
        private readonly IRepository<entity.Address> _addressRepository;
        public OwnerService(            
            IOwnerRepository ownerRepository,
            IRepository<entity.Address> addressRepository)
        {            
            _ownerRepository = ownerRepository;
            _addressRepository = addressRepository;
        }

        public async Task<Guid> CreateOwner(model.Owner ownerModel)
        {
            //Column does not allow nulls, so FullName / LastName needs to be cleaned up
            var addressDB = new entity.Address()
            {
                AddressLineOne = ownerModel.Address.AddressLineOne,
                AddressLineTwo = ownerModel.Address.AddressLineTwo,
                TownOrCity = ownerModel.Address.TownOrCity,
                County = ownerModel.Address.County,
                CountryName = ownerModel.Address.CountryName,
                AddressType = AddressType.Owner.ToString(),
                PostCode = ownerModel.Address.PostCode,
                IsActive = true,
                CreatedBy = ownerModel.Address.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _addressRepository.Add(addressDB);
            await _addressRepository.SaveChanges();

            var ownerDB = new entity.Owner()
            {
                 Email = ownerModel.Email,
                 FullName = ownerModel.FullName,
                 Telephone = ownerModel.Telephone,
                 AddressId = addressDB.Id,                 
                 CreatedBy = ownerModel.CreatedBy,
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

        public async Task<Owner> GetOwnerByEmail(string ownerEmail)
        {
            if (string.IsNullOrEmpty(ownerEmail))
            {
                throw new UserFunctionException("Invalid Owner Email Address");
            }

            return await _ownerRepository.GetOwnerByEmail(ownerEmail);
        }

        public async Task<model.Owner> GetOwnerModel(Stream userStream)
        {
            string owner = await new StreamReader(userStream).ReadToEndAsync();

            try
            {
                model.Owner? ownerModel = JsonSerializer.Deserialize<model.Owner>(owner, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return ownerModel!;
            }

            catch
            {
                throw new UserFunctionException("Cannot create Owner as Owner Model Cannot be Deserialized");
            }
        }
    }
}
