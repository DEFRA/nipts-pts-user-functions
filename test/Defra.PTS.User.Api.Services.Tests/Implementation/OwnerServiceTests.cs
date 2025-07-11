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
        private readonly Mock<IOwnerRepository> _ownerRepository = new();
        private readonly Mock<IRepository<Entity.Address>> _repoAddressService = new();
        OwnerService? sut;

        [TearDown]
        public void TearDown()
        {
            _ownerRepository.Reset();
            _repoAddressService.Reset();
        }

        #region Existing Tests - Updated

        [Test]
        public async Task CreateOwner_WhenValidData_ReturnsGuid()
        {
            Guid addressGuid = Guid.NewGuid();
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

            _repoAddressService.Setup(a => a.Add(It.IsAny<Entity.Address>())).Returns(Task.CompletedTask);
            _repoAddressService.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            Guid ownerGuid = Guid.NewGuid();

            _ownerRepository.Setup(a => a.Add(It.IsAny<Entity.Owner>())).Returns(Task.CompletedTask);
            _ownerRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            var result = await sut.CreateOwner(modelOwner);
            Assert.AreNotEqual(Guid.Empty, result);
            _ownerRepository.Verify(a => a.Add(It.IsAny<Entity.Owner>()), Times.Once);
        }

        [Test]
        public void DoesOwnerExists_WhenInValidData_ReturnsError()
        {
            sut = new OwnerService(_ownerRepository!.Object, _repoAddressService!.Object);
            var expectedResult = $"Invalid Owner Email Address";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.DoesOwnerExists(""));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);
        }

        [Test]
        public async Task DoesOwnerExists_WhenValidData_ReturnsTrue()
        {
            _ownerRepository.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);
            var result = await sut.DoesOwnerExists("cuan@test.com");
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetOwnerModel_WhenValidData_ReturnsModel()
        {
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

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
            var result = await sut.GetOwnerModel(memoryStream);
            Assert.IsNotNull(result);
            Assert.AreEqual("cuan@test.com", result.Email);
        }

        [Test]
        public async Task GetOwnerByEmail_WhenValidData_ReturnsOwner()
        {
            Guid ownerGuid = Guid.NewGuid();
            var owner = new Entity.Owner
            {
                Id = ownerGuid,
                FullName = "Cuan Brown",
                Email = "cuan@test.com",
                Telephone = "9999999999",
                CreatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Parse("FB4ECAEA-877C-4560-EDE4-08DBD163F0B6"),
                UpdatedOn = DateTime.Now,
            };
            _ownerRepository.Setup(a => a.GetOwnerByEmail(It.IsAny<string>())).Returns(Task.FromResult(owner)!);
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            var result = await sut.GetOwnerByEmail("cuan@test.com");
            Assert.IsNotNull(result);
            Assert.AreEqual("cuan@test.com", result!.Email);
        }

        [Test]
        public void GetOwnerByEmail_ThrowException()
        {
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerByEmail(""));

            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid Owner Email Address", result!.Message);
        }

        [Test]
        public void GetOwnerModel_ThrowException()
        {
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerModel(memoryStream));
            Assert.IsNotNull(result);
            Assert.AreEqual("Cannot create Owner as Owner Model Cannot be Deserialized", result?.Message);
        }

        #endregion

        #region New Tests for Email Update Functionality (AC6)

        [Test]
        public async Task UpdateOwnerEmailsByOldEmail_WhenValidEmails_UpdatesOwners()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";
            var owners = new List<Entity.Owner>
            {
                new Entity.Owner { Id = Guid.NewGuid(), Email = oldEmail, FullName = "Owner 1" },
                new Entity.Owner { Id = Guid.NewGuid(), Email = oldEmail, FullName = "Owner 2" }
            };

            _ownerRepository.Setup(a => a.GetOwnersByEmailAsync(oldEmail))
                .ReturnsAsync(owners);
            _ownerRepository.Setup(a => a.Update(It.IsAny<Entity.Owner>()));
            _ownerRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            await sut.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail);

            // Assert
            Assert.AreEqual(newEmail, owners[0].Email);
            Assert.AreEqual(newEmail, owners[1].Email);
            Assert.AreEqual(DateTime.UtcNow.Date, owners[0].UpdatedOn?.Date);
            Assert.AreEqual(DateTime.UtcNow.Date, owners[1].UpdatedOn?.Date);

            _ownerRepository.Verify(a => a.GetOwnersByEmailAsync(oldEmail), Times.Once);
            _ownerRepository.Verify(a => a.Update(It.IsAny<Entity.Owner>()), Times.Exactly(2));
            _ownerRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task UpdateOwnerEmailsByOldEmail_WhenNoOwnersFound_DoesNotUpdate()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";
            var emptyOwnerList = new List<Entity.Owner>();

            _ownerRepository.Setup(a => a.GetOwnersByEmailAsync(oldEmail))
                .ReturnsAsync(emptyOwnerList);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            await sut.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail);

            // Assert
            _ownerRepository.Verify(a => a.GetOwnersByEmailAsync(oldEmail), Times.Once);
            _ownerRepository.Verify(a => a.Update(It.IsAny<Entity.Owner>()), Times.Never);
            _ownerRepository.Verify(a => a.SaveChanges(), Times.Never);
        }

        [Test]
        public async Task UpdateOwnerEmailsByOldEmail_WhenEmptyEmails_DoesNothing()
        {
            // Arrange
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            await sut.UpdateOwnerEmailsByOldEmail("", "new@example.com");
            await sut.UpdateOwnerEmailsByOldEmail("old@example.com", "");
            await sut.UpdateOwnerEmailsByOldEmail("", "");

            // Assert
            _ownerRepository.Verify(a => a.GetOwnersByEmailAsync(It.IsAny<string>()), Times.Never);
            _ownerRepository.Verify(a => a.Update(It.IsAny<Entity.Owner>()), Times.Never);
            _ownerRepository.Verify(a => a.SaveChanges(), Times.Never);
        }

        [Test]
        public async Task UpdateOwnerEmailsByOldEmail_WhenOwnersListIsNull_DoesNotThrow()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            _ownerRepository.Setup(a => a.GetOwnersByEmailAsync(oldEmail))
                .ReturnsAsync((List<Entity.Owner>?)null);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await sut.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail));
            _ownerRepository.Verify(a => a.GetOwnersByEmailAsync(oldEmail), Times.Once);
            _ownerRepository.Verify(a => a.Update(It.IsAny<Entity.Owner>()), Times.Never);
        }

        [Test]
        public async Task UpdateOwnerEmailsByOldEmail_WhenSingleOwner_UpdatesCorrectly()
        {
            // Arrange
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";
            var owner = new Entity.Owner
            {
                Id = Guid.NewGuid(),
                Email = oldEmail,
                FullName = "Single Owner",
                UpdatedOn = DateTime.UtcNow.AddDays(-1) // Set old date
            };
            var owners = new List<Entity.Owner> { owner };

            _ownerRepository.Setup(a => a.GetOwnersByEmailAsync(oldEmail))
                .ReturnsAsync(owners);
            _ownerRepository.Setup(a => a.Update(It.IsAny<Entity.Owner>()));
            _ownerRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            await sut.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail);

            // Assert
            Assert.AreEqual(newEmail, owner.Email);
            Assert.AreEqual(DateTime.UtcNow.Date, owner.UpdatedOn?.Date);
            _ownerRepository.Verify(a => a.Update(owner), Times.Once);
            _ownerRepository.Verify(a => a.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task CreateOwner_WhenAddressCreationSucceeds_CreatesOwnerWithCorrectAddressId()
        {
            // Arrange
            var addressId = Guid.NewGuid();
            var modelAddress = new Model.Address()
            {
                AddressLineOne = "123 Test Street",
                TownOrCity = "Test City",
                PostCode = "TE1 2ST"
            };

            var modelOwner = new Model.Owner
            {
                FullName = "Test Owner",
                Email = "test@owner.com",
                Address = modelAddress
            };

            // Mock address creation to return specific ID
            _repoAddressService.Setup(a => a.Add(It.IsAny<Entity.Address>()))
                .Callback<Entity.Address>(addr => addr.Id = addressId)
                .Returns(Task.CompletedTask);
            _repoAddressService.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            _ownerRepository.Setup(a => a.Add(It.IsAny<Entity.Owner>())).Returns(Task.CompletedTask);
            _ownerRepository.Setup(a => a.SaveChanges()).ReturnsAsync(1);

            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            var result = await sut.CreateOwner(modelOwner);

            // Assert
            _ownerRepository.Verify(a => a.Add(It.Is<Entity.Owner>(o =>
                o.Email == "test@owner.com" &&
                o.FullName == "Test Owner" &&
                o.AddressId == addressId)), Times.Once);
        }

        [Test]
        public void DoesOwnerExists_WhenNullEmail_ThrowsException()
        {
            // Arrange
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act & Assert
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.DoesOwnerExists(null!));
            Assert.AreEqual("Invalid Owner Email Address", result!.Message);
        }

        [Test]
        public void GetOwnerByEmail_WhenNullEmail_ThrowsException()
        {
            // Arrange
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act & Assert
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut.GetOwnerByEmail(null!));
            Assert.AreEqual("Invalid Owner Email Address", result!.Message);
        }

        [Test]
        public async Task GetOwnerModel_WhenComplexValidJson_ReturnsCorrectModel()
        {
            // Arrange
            var json = @"{
                ""FullName"": ""John Doe"",
                ""Email"": ""john.doe@example.com"",
                ""Telephone"": ""1234567890"",
                ""CreatedBy"": ""12345678-1234-1234-1234-123456789012"",
                ""Address"": {
                    ""AddressLineOne"": ""123 Main St"",
                    ""TownOrCity"": ""Test City"",
                    ""PostCode"": ""TE1 2ST""
                }
            }";

            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            sut = new OwnerService(_ownerRepository.Object, _repoAddressService.Object);

            // Act
            var result = await sut.GetOwnerModel(memoryStream);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.FullName);
            Assert.AreEqual("john.doe@example.com", result.Email);
            Assert.AreEqual("1234567890", result.Telephone);
        }

        #endregion
    }
}