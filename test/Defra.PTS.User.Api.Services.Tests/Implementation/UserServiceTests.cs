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

namespace Defra.PTS.User.Api.Services.Tests.Implementation
{
    [TestFixture]
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IRepository<Entity.Address>> _repoAddressService = new();
        UserService? sut;

        [TearDown]
        public void TearDown()
        {
            _userRepository.Reset();
            _repoAddressService.Reset();
        }

        [Test]
        public async Task CreateUser_WhenValidData_ReturnsGuid() 
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
                AddressType = AddressType.User.ToString(),

                IsActive = true,
                CreatedBy = Guid.Parse("AB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };

            var modelUser = new Model.User
            {
                Address = modelAddress,
                Email = "cuan@test.com",
                FullName = "Cuan Brown",
                FirstName = "Cuan",
                LastName = "Brown",
                AddressId = addressGuid,
                Telephone = "9999999999",
                ContactId = Guid.Parse("EB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                Uniquereference = "123",
                SignInDateTime = DateTime.Now,
                SignOutDateTime = DateTime.Now,
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };


            
            var address = new Entity.Address()
            {
                AddressLineOne = "19 First Avenue",
                AddressLineTwo = "",
                TownOrCity = "Grays",
                County = "Essex",
                CountryName = "UK",
                PostCode = "RM13 4FT",
                AddressType = AddressType.User.ToString(),
                IsActive = true,
                CreatedBy = Guid.Parse("AB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };
            _repoAddressService.Setup(a => a.Add(address)).Returns(Task.FromResult(address.Id));
            await _repoAddressService.Object.Add(address);
            _repoAddressService.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            Guid userGuid = Guid.Empty;
            var user = new Entity.User
            {
                Id = userGuid,
                Email = "cuan@test.com",
                FullName = "Cuan Brown",
                FirstName = "Cuan",
                LastName = "Brown",
                AddressId = addressGuid,
                Telephone = "9999999999",
                ContactId = Guid.Parse("EB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                Uniquereference = "123",
                SignInDateTime = DateTime.Now,
                SignOutDateTime = DateTime.Now,
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };

            _userRepository.Setup(a => a.Add(user)).Returns(Task.FromResult(user.Id));

            await _userRepository.Object.Add(user);
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);
            

            sut = new UserService(_userRepository.Object);

            var result = sut.CreateUser(modelUser);
            Assert.AreEqual(userGuid, result.Result);
            _userRepository.Verify(a => a.Add(user), Times.Once);
        }

        [Test]
        public void UpdateUser_WhenValidData_ReturnsGuid()
        {
            
            Guid addressGuid = Guid.Empty;

            Guid userGuid = Guid.Empty;
            var user = new Entity.User
            {
                Id = userGuid,
                Email = "cuan@test.com",
                FullName = "Cuan Brown",
                FirstName = "Cuan",
                LastName = "Brown",
                AddressId = addressGuid,
                Telephone = "9999999999",
                ContactId = Guid.Parse("EB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                Uniquereference = "123",
                SignInDateTime = DateTime.Now,
                SignOutDateTime = DateTime.Now,
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };
            _userRepository.Setup(a => a.GetUser(It.IsAny<string>())).Returns(Task.FromResult(user)!);
            _userRepository.Setup(a => a.Update(user));            
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);


            sut = new UserService(_userRepository.Object);

            var result = sut.UpdateUser("cuan@test.com", "signin");
            Assert.AreEqual(userGuid, result.Result);
            _userRepository.Verify(a => a.Update(user), Times.Once);
            _userRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateUser_WhenValidData_UserSignOut_ReturnsGuid()
        {

            Guid addressGuid = Guid.Empty;

            Guid userGuid = Guid.Empty;
            var user = new Entity.User
            {
                Id = userGuid,
                Email = "cuan@test.com",
                FullName = "Cuan Brown",
                FirstName = "Cuan",
                LastName = "Brown",
                AddressId = addressGuid,
                Telephone = "9999999999",
                ContactId = Guid.Parse("EB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                Uniquereference = "123",
                SignInDateTime = DateTime.Now,
                SignOutDateTime = DateTime.Now,
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now
            };
            _userRepository.Setup(a => a.GetUser(It.IsAny<string>())).Returns(Task.FromResult(user)!);
            _userRepository.Setup(a => a.Update(user));
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);


            sut = new UserService(_userRepository.Object);

            var result = sut.UpdateUser("cuan@test.com", "signout");
            Assert.AreEqual(userGuid, result.Result);
            _userRepository.Verify(a => a.Update(user), Times.Once);
            _userRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void DoesUserExists_WhenInValidData_ReturnsError()
        {          
            sut = new UserService(_userRepository.Object);            
            var expectedResult = $"Invalid User Email Address";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.DoesUserExists(""));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);
        }

        [Test]
        public async Task DoesUserExists_WheValidData_ReturnsError()
        {   
            _userRepository.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            sut = new UserService(_userRepository.Object);
            var result = await sut.DoesUserExists("cuan@test.com");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetUserModel_WhenValidData_ReturnsModel()
        {            
            sut = new UserService(_userRepository.Object);
   
            var json = "{" +
                    "\"id\":\"00000000-0000-0000-0000-000000000000\"," +
                    "\"FullName\":null," +
                    "\"Email\":null," +
                    "\"FirstName\":\"Cuan\"," +
                    "\"LastName\":\"Brown\"," +
                    "\"AddressId\":null," +
                    "\"Role\":\"test\"," +
                    "\"Telephone\":null," +
                    "\"ContactId\":null," +
                    "\"SignInDateTime\":null," +
                    "\"SignOutDateTime\":null," +
                    "\"CreatedBy\":null," +
                    "\"CreatedOn\":null," +
                    "\"UpdatedBy\":null," +
                    "\"UpdatedOn\":null," +
                    "\"Address\":null" +
                "}";

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            sut = new UserService(_userRepository.Object);
            var result = await sut.GetUserModel(memoryStream);
            Assert.IsNotNull(result);
            Assert.AreEqual("Cuan", result.FirstName);
            Assert.AreEqual("Brown", result.LastName);
        }

        [Test]
        public void GetUserModel_ThrowException()
        {
            sut = new UserService(_userRepository.Object);

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));
            sut = new UserService(_userRepository.Object);

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetUserModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create User as User Model Cannot be Deserialized", result!.Message);
        }

        [Test]
        public void GetUserEmailModel_WhenValidData_ReturnsModel()
        {
            sut = new UserService(_userRepository.Object);

            var json = "{" +
                 "\"Email\":\"tt@tt.com\"," +
                    "\"Type\":null" +

                "}";

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            sut = new UserService(_userRepository.Object);
            var result = sut.GetUserEmailModel(memoryStream);
            Assert.IsNotNull(result);
            Assert.AreEqual("tt@tt.com", result.Result.Email);           
        }

        [Test]
        public void GetUserEmailModel_ThrowException()
        {
            sut = new UserService(_userRepository.Object);            

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));
            sut = new UserService(_userRepository.Object);

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetUserEmailModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create User as UserEmail Model Cannot be Deserialized", result!.Message);
        }
    }
}
