using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models;
using Defra.PTS.User.Models.CustomException;
using Defra.PTS.User.Models.Enums;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Entity = Defra.PTS.User.Entities;
using Model = Defra.PTS.User.Models;

namespace Defra.PTS.User.ApiServices.Implementation
{
    
    public class AddressService : IAddressService
    {        
        private readonly IRepository<Entity.Address> _addressRepository;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public AddressService(
             IRepository<Entity.Address> addressRepository)
        {
            _addressRepository = addressRepository;            
        }


        public async Task<Guid> CreateAddress(Address addressModel)
        {
            var addressDB = new Entity.Address()
            {
                AddressLineOne = addressModel.AddressLineOne,
                AddressLineTwo = addressModel.AddressLineTwo,
                TownOrCity = addressModel.TownOrCity,
                County = addressModel.County,
                CountryName = addressModel.CountryName,
                AddressType = addressModel.AddressType,
                PostCode = addressModel.PostCode,
                IsActive = true,
                CreatedBy = addressModel.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _addressRepository.Add(addressDB);
            await _addressRepository.SaveChanges();

            return addressDB.Id;
        }

        public async Task<Model.Address> GetAddressModel(Stream addressStream)
        {
            string address = await new StreamReader(addressStream).ReadToEndAsync();

            try
            {
                Model.Address? addressModel = JsonSerializer.Deserialize<Model.Address>(address, _jsonOptions);
                return addressModel!;
            }
            catch
            {
                throw new UserFunctionException("Cannot create Address as Address Model Cannot be Deserialized");
            }
        }
    }
}
