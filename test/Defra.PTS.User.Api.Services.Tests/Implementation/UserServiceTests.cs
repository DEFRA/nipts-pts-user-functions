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

        #region Existing Tests - Updated for New Architecture

        [Test]
        public async Task CreateUser_WhenValidData_ReturnsGuid()
        {
            // Arrange
            Guid addressGuid = Guid.NewGuid(); 
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

            _userRepository.Setup(a => a.Add(It.IsAny<Entity.User>()))
                .Callback<Entity.User>(user => user.Id = Guid.NewGuid()) 
                .Returns(Task.CompletedTask);
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.CreateUser(modelUser);

            // Assert
            Assert.AreNotEqual(Guid.Empty, result);
            _userRepository.Verify(a => a.Add(It.IsAny<Entity.User>()), Times.Once);
            _userRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateUser_WhenValidData_ReturnsGuid()
        {
            Guid addressGuid = Guid.Empty;
            Guid userGuid = Guid.NewGuid();
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
            _userRepository.Setup(a => a.Update(It.IsAny<Entity.User>()));
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new UserService(_userRepository.Object);

            var result = sut.UpdateUser("cuan@test.com", "signin");
            Assert.AreEqual(userGuid, result.Result);
            _userRepository.Verify(a => a.Update(It.IsAny<Entity.User>()), Times.Once);
            _userRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateUser_WhenValidData_UserSignOut_ReturnsGuid()
        {
            Guid addressGuid = Guid.Empty;
            Guid userGuid = Guid.NewGuid();
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
            _userRepository.Setup(a => a.Update(It.IsAny<Entity.User>()));
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new UserService(_userRepository.Object);

            var result = sut.UpdateUser("cuan@test.com", "signout");
            Assert.AreEqual(userGuid, result.Result);
            _userRepository.Verify(a => a.Update(It.IsAny<Entity.User>()), Times.Once);
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
        public async Task DoesUserExists_WheValidData_ReturnsTrue()
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

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetUserModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create User as User Model Cannot be Deserialized", result!.Message);
        }

        [Test]
        public async Task GetUserEmailModel_WhenValidData_ReturnsModel()
        {
            sut = new UserService(_userRepository.Object);

            var json = "{" +
                 "\"Email\":\"tt@tt.com\"," +
                    "\"Type\":null" +
                "}";

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            var result = await sut.GetUserEmailModel(memoryStream);
            Assert.IsNotNull(result);
            Assert.AreEqual("tt@tt.com", result.Email);
        }

        [Test]
        public void GetUserEmailModel_ThrowException()
        {
            sut = new UserService(_userRepository.Object);

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetUserEmailModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create User as UserEmail Model Cannot be Deserialized", result!.Message);
        }

        #endregion

        #region New Tests for ContactId-Based Functionality

        [Test]
        public async Task GetUserByContactId_WhenValidContactId_ReturnsUser()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var expectedUser = new Entity.User
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                Email = "test@example.com",
                FullName = "Test User"
            };

            _userRepository.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(expectedUser);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.GetUserByContactId(contactId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUser.Id, result?.Id);
            Assert.AreEqual(contactId, result?.ContactId);
            _userRepository.Verify(a => a.GetUserByContactId(contactId), Times.Once);
        }

        [Test]
        public async Task GetUserByContactId_WhenContactIdNotFound_ReturnsNull()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            _userRepository.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync((Entity.User?)null);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.GetUserByContactId(contactId);

            // Assert
            Assert.IsNull(result);
            _userRepository.Verify(a => a.GetUserByContactId(contactId), Times.Once);
        }

        [Test]
        public async Task DoesUserExistsByContactId_WhenUserExists_ReturnsTrue()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            _userRepository.Setup(a => a.DoesUserExistsByContactId(contactId))
                .ReturnsAsync(true);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.DoesUserExistsByContactId(contactId);

            // Assert
            Assert.IsTrue(result);
            _userRepository.Verify(a => a.DoesUserExistsByContactId(contactId), Times.Once);
        }

        [Test]
        public async Task DoesUserExistsByContactId_WhenUserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            _userRepository.Setup(a => a.DoesUserExistsByContactId(contactId))
                .ReturnsAsync(false);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.DoesUserExistsByContactId(contactId);

            // Assert
            Assert.IsFalse(result);
            _userRepository.Verify(a => a.DoesUserExistsByContactId(contactId), Times.Once);
        }

        [Test]
        public async Task UpdateUserEmail_WhenValidData_UpdatesUserEmail()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";
            var user = new Entity.User
            {
                Id = Guid.NewGuid(),
                Email = oldEmail,
                FullName = "Test User"
            };

            _userRepository.Setup(a => a.GetUser(oldEmail))
                .ReturnsAsync(user);
            _userRepository.Setup(a => a.Update(It.IsAny<Entity.User>()));
            _userRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new UserService(_userRepository.Object);

            // Act
            await sut.UpdateUserEmail(oldEmail, newEmail);

            // Assert
            Assert.AreEqual(newEmail, user.Email);
            Assert.AreEqual(DateTime.UtcNow.Date, user.UpdatedOn?.Date);
            _userRepository.Verify(a => a.GetUser(oldEmail), Times.Once);
            _userRepository.Verify(a => a.Update(user), Times.Once);
            _userRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public void  UpdateUserEmail_WhenUserNotFound_DoesNotThrow()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            _userRepository.Setup(a => a.GetUser(oldEmail))
                .ReturnsAsync((Entity.User?)null);

            sut = new UserService(_userRepository.Object);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await sut.UpdateUserEmail(oldEmail, newEmail));
            _userRepository.Verify(a => a.GetUser(oldEmail), Times.Once);
            _userRepository.Verify(a => a.Update(It.IsAny<Entity.User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserEmail_WhenEmptyEmails_DoesNothing()
        {
            // Arrange
            sut = new UserService(_userRepository.Object);

            // Act
            await sut.UpdateUserEmail("", "new@example.com");
            await sut.UpdateUserEmail("old@example.com", "");
            await sut.UpdateUserEmail("", "");

            // Assert
            _userRepository.Verify(a => a.GetUser(It.IsAny<string>()), Times.Never);
            _userRepository.Verify(a => a.Update(It.IsAny<Entity.User>()), Times.Never);
        }

        [Test]
        public async Task GetOwnerEmailUpdateModel_WhenValidData_ReturnsModel()
        {
            // Arrange
            var json = @"{
                ""oldEmail"": ""old@example.com"",
                ""newEmail"": ""new@example.com"",
                ""userId"": ""12345678-1234-1234-1234-123456789012""
            }";

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.GetOwnerEmailUpdateModel(memoryStream);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("old@example.com", result.OldEmail);
            Assert.AreEqual("new@example.com", result.NewEmail);
            Assert.AreEqual(Guid.Parse("12345678-1234-1234-1234-123456789012"), result.UserId);
        }

        [Test]
        public void GetOwnerEmailUpdateModel_WhenInvalidJson_ThrowsException()
        {
            // Arrange
            var invalidJson = "invalid json";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));
            sut = new UserService(_userRepository.Object);

            // Act & Assert
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerEmailUpdateModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot deserialize OwnerEmailUpdateModel", result!.Message);
        }

        [Test]
        public async Task GetUserIdAsync_WhenValidEmail_ReturnsUserId()
        {
            // Arrange
            var email = "test@example.com";
            var userId = Guid.NewGuid();
            var user = new Entity.User { Id = userId, Email = email };

            _userRepository.Setup(a => a.GetUser(email))
                .ReturnsAsync(user);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.GetUserIdAsync(email);

            // Assert
            Assert.AreEqual(userId, result);
            _userRepository.Verify(a => a.GetUser(email), Times.Once);
        }

        [Test]
        public async Task PerformHealthCheckLogic_CallsRepository()
        {
            // Arrange
            _userRepository.Setup(a => a.PerformHealthCheckLogic())
                .ReturnsAsync(true);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.PerformHealthCheckLogic();

            // Assert
            Assert.IsTrue(result);
            _userRepository.Verify(a => a.PerformHealthCheckLogic(), Times.Once);
        }

        [Test]
        public async Task GetUserDetail_WhenValidContactId_ReturnsUserDetail()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var expectedUserDetail = new Entity.UserDetail
            {
                FullName = "Test User",
                Email = "test@example.com"
            };

            _userRepository.Setup(a => a.GetUserDetail(contactId))
                .ReturnsAsync(expectedUserDetail);

            sut = new UserService(_userRepository.Object);

            // Act
            var result = await sut.GetUserDetail(contactId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUserDetail.FullName, result.FullName);
            Assert.AreEqual(expectedUserDetail.Email, result.Email);
            _userRepository.Verify(a => a.GetUserDetail(contactId), Times.Once);
        }

        #endregion
    }
}