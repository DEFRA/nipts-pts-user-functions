using Defra.PTS.User.ApiServices.Implementation;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Entity = Defra.PTS.User.Entities;
using Model = Defra.PTS.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.PTS.User.Repositories.Implementation;
using Defra.PTS.User.Models.CustomException;
using Defra.PTS.User.Models.Enums;
using System.Text.Json;
using Newtonsoft.Json;

namespace Defra.PTS.Owner.Api.Services.Tests.Implementation
{
    [TestFixture]
    public class OwnerServiceTests
    {                
        private readonly Mock<IOwnerRepository> _OwnerRepository = new();
        private readonly Mock<IRepository<Entity.Address>> _repoAddressService = new();
        OwnerService? sut;

        [Test]
        public async Task CreateOwner_WhenValidData_ReturnsGuid() 
        {
            Guid addressGuid = Guid.Empty;
            var modelAddress = new Model.Address()
            {
                AddressLineOne = "19 First Avenue",
                AddressLineTwo = "",                
                TownOrCity = "Grays",
                County = "Essex",
                CountryName = "UK",
                PostCode = "RM13 4FT",
                
                IsActive = true,
                CreatedBy = Guid.Parse("AB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };

            var modelOwner = new Model.Owner
            { 
                FullName = "Cuan Brown",
                Email = "cuan@test.com",
                Telephone = "9999999999",                
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                UpdatedOn = DateTime.Now,
                Address = modelAddress,
            };

        var address = new Entity.Address()
            {
                AddressLineOne = "19 First Avenue",
                AddressLineTwo = "",
                TownOrCity = "Grays",
                County = "Essex",
                CountryName = "UK",
                PostCode = "RM13 4FT",
                AddressType = AddressType.Owner.ToString(),
                IsActive = true,
                CreatedBy = Guid.Parse("AB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };
            _repoAddressService.Setup(a => a.Add(address)).Returns(Task.FromResult(address.Id = addressGuid));
            await _repoAddressService.Object.Add(address);
            _repoAddressService.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            Guid OwnerGuid = Guid.Empty;
            var Owner = new Entity.Owner
            {
                Id = OwnerGuid,
                Email = "cuan@test.com",
                FullName = "Cuan Brown",                
                Telephone = "9999999999",
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                UpdatedOn = DateTime.Now,
                OwnerType = "Test", 
                CharityName = "TestCharity"
            };

            _OwnerRepository.Setup(a => a.Add(Owner)).Returns(Task.FromResult(Owner.Id = OwnerGuid));

            await _OwnerRepository.Object.Add(Owner);
            _OwnerRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);
            

            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);

            var result = sut.CreateOwner(modelOwner);
            Assert.AreEqual(OwnerGuid, result.Result);
            _OwnerRepository.Verify(a => a.Add(Owner), Times.Once);
        }

        [Test]
        public void DoesOwnerExists_WhenInValidData_ReturnsError()
        {                     
            sut = new OwnerService(_OwnerRepository!.Object, _repoAddressService!.Object);
            var expectedResult = $"Invalid Owner Email Address";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.DoesOwnerExists(""));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);
        }

        [Test]
        public void DoesOwnerExists_WheValidData_ReturnsError()
        {            
            _OwnerRepository.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);
            var result = sut.DoesOwnerExists("cuan@test.com");
            Assert.IsTrue(result.Result);
        }


        [Test]
        public void GetOwnerModel_WhenValidData_ReturnsModel()
        {                      
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);

            var json = "{" +                    
                    "\"FullName\":null," +
                    "\"Email\":\"cuan@test.com\"," +
                    "\"OwnerTypeId\":1," +                    
                    "\"Telephone\":null," +
                    "\"CreatedBy\":null," +
                    "\"CreatedOn\":null," +
                    "\"UpdatedBy\":null," +
                    "\"UpdatedOn\":null" +                    
                "}";

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);
            var result = sut.GetOwnerModel(memoryStream);
            Assert.IsNotNull(result);
            Assert.AreEqual("cuan@test.com", result.Result.Email);
        }


        [Test]
        public void GetOwnerEmailModel_WhenValidData_ReturnsModel()
        {            
            Guid OwnerGuid = Guid.Empty;
            var owner = new Entity.Owner
            {
                Id = OwnerGuid,
                FullName = "Cuan Brown",
                Email = "cuan@test.com",
                Telephone = "9999999999",
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                UpdatedOn = DateTime.Now,

            };
            _OwnerRepository.Setup(a => a.GetOwnerByEmail(It.IsAny<string>())).Returns(Task.FromResult(owner)!);
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);

            var result = sut.GetOwnerByEmail("cuan@test.com");
            Assert.IsNotNull(result);
            Assert.AreEqual("cuan@test.com", result.Result!.Email);
        }

        [Test]
        public void GetOwnerByEmail_ThrowException()
        {
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerByEmail(""));

            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Owner Email Address", result!.Message);
        }

        [Test]
        public void GetOwnerModel_ThrowException()
        {
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));
            sut = new OwnerService(_OwnerRepository.Object, _repoAddressService.Object);

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create Owner as Owner Model Cannot be Deserialized", result?.Message);
        }
    }
}
