using Model = Defra.PTS.User.Models;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using testFunc = Defra.PTS.User.Functions.Functions.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Defra.PTS.User.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Entity = Defra.PTS.User.Entities;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{
    public class UserTest
    {
        private readonly Mock<HttpRequest> requestMock = new();
        private readonly Mock<ILogger> loggerMock = new();
        private readonly Mock<IUserService> userServiceMock = new();
        private readonly Mock<IOwnerService> ownerServiceMock = new();
        testFunc.User? sut;

        [SetUp]
        public void SetUp()
        {
            sut = new testFunc.User(userServiceMock.Object, ownerServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            requestMock.Reset();
            loggerMock.Reset();
            userServiceMock.Reset();
            ownerServiceMock.Reset();
        }

       

        [Test]
        public void CreateUser_WhenRequestDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NULL or Empty";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(null, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public void CreateUser_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NULL or Empty";

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(requestMock.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public async Task CreateUser_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new Model.User() { Email = "test@example.com" }));
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            userServiceMock.Setup(a => a.CreateUser(It.IsAny<Model.User>())).Returns(guid);

            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Once);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Once);
        }

        [Test]
        public async Task CreateUser_WhenRequestBodyExistsAndUserExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new Model.User() { Email = "test@example.com" }));
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            userServiceMock.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);
            userServiceMock.Setup(a => a.GetUserIdAsync(It.IsAny<string>())).Returns(guid);

            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Once);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public void UpdateUser_WhenRequestDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUser(null, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public void UpdateUser_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUser(requestMock.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUser_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>())).Returns(Task.FromResult(new Model.UserEmail() { Email = "salim@test.co.uk", Type = "signin" }));
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            userServiceMock.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);

            var result = await sut!.UpdateUser(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserEmailModel(It.IsAny<Stream>()), Times.Once);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMock.Verify(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task UpdateUser_WhenRequestBodyExists_Then_ReturnsErrorMessage()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = $"Cannot update new User as user does not exists";
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>())).Returns(Task.FromResult(new Model.UserEmail() { Email = "salim@test.co.uk", Type = "signin" }));
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            userServiceMock.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);

            var result = await sut!.UpdateUser(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserEmailModel(It.IsAny<Stream>()), Times.Once);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMock.Verify(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void UpdateUserAddress_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUserAddress(requestMock.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMock.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.CreateUser(It.IsAny<Model.User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAddress_UserDoesNotExist()
        {
            var expectedResult = "Cannot update new User as user does not exists";

            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            requestMock.Setup(a => a.Body).Returns(memoryStream);
            userServiceMock.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>()))
                .ReturnsAsync(new UserEmail() { Email = "test@example.com", Type = "signin" });
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await sut!.UpdateUserAddress(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);
        }

        [Test]
        public async Task UpdateUserAddress()
        {
            var userId = Guid.NewGuid();

            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            requestMock.Setup(a => a.Body).Returns(memoryStream);
            userServiceMock.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>()))
                .ReturnsAsync(new UserEmail() { Email = "test@email.com", Type = "signin" });
            userServiceMock.Setup(a => a.DoesUserExists(It.IsAny<string>()))
                .ReturnsAsync(true);
            userServiceMock.Setup(x => x.UpdateUser(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(userId);

            var result = await sut!.UpdateUserAddress(requestMock.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(userId, okResult?.Value);
        }

        [Test]
        public async Task CreateUser_ContactIdExists_EmailUnchanged_ReturnsExistingUserId()
        {
            // Arrange - AC2: Single Record (existing user, no email change)
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();
            var email = "test@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = email
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);
            userServiceMock.Setup(a => a.UpdateUser(email, "signin"))
                .ReturnsAsync(existingUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserByContactId(contactId), Times.Once);
            userServiceMock.Verify(a => a.UpdateUser(email, "signin"), Times.Once);
            userServiceMock.Verify(a => a.UpdateUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            ownerServiceMock.Verify(a => a.UpdateOwnerEmailsByOldEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateUser_ContactIdExists_EmailChanged_UpdatesEmailsAndReturnsUserId()
        {
            // Arrange - AC4: Single Record (existing user, email change)
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = newEmail
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = oldEmail
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);
            userServiceMock.Setup(a => a.UpdateUserEmail(oldEmail, newEmail))
                .Returns(Task.CompletedTask);
            ownerServiceMock.Setup(a => a.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail))
                .Returns(Task.CompletedTask);
            userServiceMock.Setup(a => a.UpdateUser(newEmail, "signin"))
                .ReturnsAsync(existingUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserByContactId(contactId), Times.Once);
            userServiceMock.Verify(a => a.UpdateUserEmail(oldEmail, newEmail), Times.Once);
            ownerServiceMock.Verify(a => a.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail), Times.Once);
            userServiceMock.Verify(a => a.UpdateUser(newEmail, "signin"), Times.Once);
        }

        [Test]
        public async Task CreateUser_ContactIdNotFound_CreatesNewUser()
        {
            // Arrange - AC1: Create New User
            var contactId = Guid.NewGuid();
            var newUserId = Guid.NewGuid();
            var email = "new@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync((Entity.User?)null);
            userServiceMock.Setup(a => a.DoesUserExists(email))
                .ReturnsAsync(false);
            userServiceMock.Setup(a => a.CreateUser(userModel))
                .ReturnsAsync(newUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(newUserId, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserByContactId(contactId), Times.Once);
            userServiceMock.Verify(a => a.CreateUser(userModel), Times.Once);
            userServiceMock.Verify(a => a.UpdateUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            ownerServiceMock.Verify(a => a.UpdateOwnerEmailsByOldEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateUser_NoContactId_FallsBackToEmailLogic()
        {
            // Arrange - Backward compatibility test
            var email = "test@example.com";
            var newUserId = Guid.NewGuid();

            var userModel = new Model.User
            {
                ContactId = null, // No ContactId 
                Email = email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.DoesUserExists(email))
                .ReturnsAsync(false);
            userServiceMock.Setup(a => a.CreateUser(userModel))
                .ReturnsAsync(newUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(newUserId, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserByContactId(It.IsAny<Guid>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(email), Times.Once);
            userServiceMock.Verify(a => a.CreateUser(userModel), Times.Once);
        }

        [Test]
        public async Task CreateUser_EmailUpdateThrowsException_ContinuesExecution()
        {
            // Arrange - Error handling test
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = newEmail
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = oldEmail
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);
            userServiceMock.Setup(a => a.UpdateUserEmail(oldEmail, newEmail))
                .ThrowsAsync(new Exception("Database error"));
            userServiceMock.Setup(a => a.UpdateUser(newEmail, "signin"))
                .ReturnsAsync(existingUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert - Should still return success even if email update fails
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);

            userServiceMock.Verify(a => a.UpdateUserEmail(oldEmail, newEmail), Times.Once);
            userServiceMock.Verify(a => a.UpdateUser(newEmail, "signin"), Times.Once);
        }

        [Test]
        public async Task CreateUser_SignInUpdateThrowsException_ContinuesExecution()
        {
            // Arrange - Error handling for sign-in update
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();
            var email = "test@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = email
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);
            userServiceMock.Setup(a => a.UpdateUser(email, "signin"))
                .ThrowsAsync(new Exception("Sign-in update failed"));

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert - Should still return success even if sign-in update fails
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);
        }

        [Test]
        public void CreateUser_WhenUserModelIsNull_ThrowsException()
        {
            // Arrange
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync((Model.User?)null);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

            // Act & Assert
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(requestMock.Object, loggerMock.Object));
            Assert.AreEqual("Failed to parse user model from input data", result!.Message);
        }

        [Test]
        public void CreateUser_WhenNoContactIdAndNoEmail_ThrowsException()
        {
            // Arrange
            var userModel = new Model.User
            {
                ContactId = null,
                Email = "" // Empty email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);

            // Act & Assert
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(requestMock.Object, loggerMock.Object));
            Assert.AreEqual("User model must have either ContactId or Email", result!.Message);
        }

        [Test]
        public async Task CreateUser_ContactIdEmpty_FallsBackToEmailLogic()
        {
            // Arrange - Empty GUID should fall back to email logic
            var email = "test@example.com";
            var newUserId = Guid.NewGuid();

            var userModel = new Model.User
            {
                ContactId = Guid.Empty,
                Email = email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.DoesUserExists(email))
                .ReturnsAsync(false);
            userServiceMock.Setup(a => a.CreateUser(userModel))
                .ReturnsAsync(newUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(newUserId, okResult?.Value);

            userServiceMock.Verify(a => a.GetUserByContactId(It.IsAny<Guid>()), Times.Never);
            userServiceMock.Verify(a => a.DoesUserExists(email), Times.Once);
        }

        [Test]
        public async Task CreateUser_EmailChangedButOwnerUpdateFails_StillSucceeds()
        {            
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = newEmail
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = oldEmail
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);
            userServiceMock.Setup(a => a.UpdateUserEmail(oldEmail, newEmail))
                .Returns(Task.CompletedTask);
            ownerServiceMock.Setup(a => a.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail))
                .ThrowsAsync(new Exception("Owner update failed"));
            userServiceMock.Setup(a => a.UpdateUser(newEmail, "signin"))
                .ReturnsAsync(existingUserId);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert - Should still succeed even if owner update fails
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);

            ownerServiceMock.Verify(a => a.UpdateOwnerEmailsByOldEmail(oldEmail, newEmail), Times.Once);
        }

        [Test]
        public async Task CreateUser_EmailsAreNullOrEmpty_SkipsEmailUpdate()
        {
            
            var contactId = Guid.NewGuid();
            var existingUserId = Guid.NewGuid();

            var userModel = new Model.User
            {
                ContactId = contactId,
                Email = "" // Empty email
            };

            var existingUser = new Entity.User
            {
                Id = existingUserId,
                ContactId = contactId,
                Email = "" // Empty existing email
            };

            var json = JsonConvert.SerializeObject(userModel);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            requestMock.Setup(a => a.Body).Returns(memoryStream);

            userServiceMock.Setup(a => a.GetUserModel(It.IsAny<Stream>()))
                .ReturnsAsync(userModel);
            userServiceMock.Setup(a => a.GetUserByContactId(contactId))
                .ReturnsAsync(existingUser);

            // Act
            var result = await sut!.CreateUser(requestMock.Object, loggerMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(existingUserId, okResult?.Value);

            // Should not attempt email updates when emails are empty
            userServiceMock.Verify(a => a.UpdateUserEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            ownerServiceMock.Verify(a => a.UpdateOwnerEmailsByOldEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userServiceMock.Verify(a => a.UpdateUser(It.IsAny<string>(), "signin"), Times.Never);
        }

        
    }
}