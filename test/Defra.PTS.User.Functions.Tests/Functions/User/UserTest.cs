using model = Defra.PTS.User.Models;
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

namespace Defra.PTS.User.Functions.Tests.Functions.User
{
    public class UserTest
    {
        private readonly Mock<HttpRequest> requestMoq = new();
        private readonly Mock<ILogger> loggerMock = new();
        private readonly Mock<IUserService> userServiceMoq = new();
        testFunc.User? sut;

        [SetUp]
        public void SetUp()
        {            
            sut = new testFunc.User(userServiceMoq.Object);
        }

        [TearDown]
        public void TearDown()
        {
            requestMoq.Reset();
            loggerMock.Reset();
            userServiceMoq.Reset();
        }

        [Test]
        public void CreateUser_WhenRequestDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(null, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public void CreateUser_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";                       

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateUser(requestMoq.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public void CreateUser_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq.Setup(a => a.Body).Returns(memoryStream);


            userServiceMoq.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.User() { }));
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            userServiceMoq.Setup(a => a.CreateUser(It.IsAny<model.User>())).Returns(guid);

            var result = sut!.CreateUser(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Once);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Once);
        }

        [Test]
        public void CreateUser_WhenRequestBodyExistsAndUserExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq.Setup(a => a.Body).Returns(memoryStream);


            userServiceMoq.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.User() { }));
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            userServiceMoq.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);
            userServiceMoq.Setup(a => a.GetUserIdAsync(It.IsAny<string>())).Returns(guid);

            var result = sut!.CreateUser(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Once);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public void UpdateUser_WhenRequestDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUser(null, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public void UpdateUser_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";           

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUser(requestMoq.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public void UpdateUser_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq.Setup(a => a.Body).Returns(memoryStream);


            userServiceMoq.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.User() { }));
            userServiceMoq.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.UserEmail() { Email = "salim@test.co.uk", Type = "signin" }));
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            userServiceMoq.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);

            var result = sut!.UpdateUser(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);


            userServiceMoq.Verify(a => a.GetUserEmailModel(It.IsAny<Stream>()), Times.Once);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMoq.Verify(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdateUser_WhenRequestBodyExists_Then_ReturnsErrorMessage()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = $"Cannot update new User as user does not exists";
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq.Setup(a => a.Body).Returns(memoryStream);


            userServiceMoq.Setup(a => a.GetUserModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.User() { }));
            userServiceMoq.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>())).Returns(Task.FromResult(new model.UserEmail() { Email = "salim@test.co.uk", Type = "signin" }));
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            userServiceMoq.Setup(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(guid);

            var result = sut!.UpdateUser(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);


            userServiceMoq.Verify(a => a.GetUserEmailModel(It.IsAny<Stream>()), Times.Once);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Once);
            userServiceMoq.Verify(a => a.UpdateUser(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void UpdateUserAddress_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid user input, is NUll or Empty";            

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.UpdateUserAddress(requestMoq.Object, loggerMock.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result?.Message);

            userServiceMoq.Verify(a => a.GetUserModel(It.IsAny<Stream>()), Times.Never);
            userServiceMoq.Verify(a => a.DoesUserExists(It.IsAny<string>()), Times.Never);
            userServiceMoq.Verify(a => a.CreateUser(It.IsAny<model.User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAddress_UserDoesNotExist()
        {
            var expectedResult = "Cannot update new User as user does not exists";

            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" , \"Email\" : \"salim@test.co.uk\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            requestMoq.Setup(a => a.Body).Returns(memoryStream);
            userServiceMoq.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>()))
                .ReturnsAsync(new UserEmail() { Email = null, Type = "signin" });
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await sut!.UpdateUserAddress(requestMoq.Object, loggerMock.Object);
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

            requestMoq.Setup(a => a.Body).Returns(memoryStream);
            userServiceMoq.Setup(a => a.GetUserEmailModel(It.IsAny<Stream>()))
                .ReturnsAsync(new UserEmail() { Email = "test@email.com", Type = "signin"});
            userServiceMoq.Setup(a => a.DoesUserExists(It.IsAny<string>()))
                .ReturnsAsync(true);
            userServiceMoq.Setup(x => x.UpdateUser(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(userId);

            var result = await sut!.UpdateUserAddress(requestMoq.Object, loggerMock.Object);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(userId, okResult?.Value);
        }
    }
}
