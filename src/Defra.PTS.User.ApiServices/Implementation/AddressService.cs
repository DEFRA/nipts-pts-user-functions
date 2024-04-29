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
using entity = Defra.PTS.User.Entities;
using model = Defra.PTS.User.Models;

namespace Defra.PTS.User.ApiServices.Implementation
{
    [ExcludeFromCodeCoverage]
    public class AddressService : IAddressService
    {        
        private readonly IRepository<entity.Address> _addressRepository;

        public AddressService(
             IRepository<entity.Address> addressRepository)
        {
            _addressRepository = addressRepository;            
        }


        public async Task<Guid> CreateAddress(Address addressModel)
        {
            var addressDB = new entity.Address()
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

        public async Task<model.Address> GetAddressModel(Stream addressStream)
        {
            string address = await new StreamReader(addressStream).ReadToEndAsync();

            try
            {
                model.Address? addressModel = JsonSerializer.Deserialize<model.Address>(address, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return addressModel!;
            }
            catch
            {
                throw new UserFunctionException("Cannot create Address as Address Model Cannot be Deserialized");
            }
        }
    }
}
