using Model = Defra.PTS.User.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using testFunc = Defra.PTS.User.Functions.Functions.Owner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Identity.Client.Extensions.Msal;
using NUnit.Framework;
using Defra.PTS.User.Models.CustomException;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.User.Functions.Tests.Functions.Owner
{
    public class OwnerTest
    {
        private Mock<HttpRequest>? requestMoq;
        private Mock<ILogger>? loggerMock;
        private Mock<IOwnerService>? ownerServiceMoq;
        testFunc.Owner? sut;

        [SetUp]
        public void SetUp()
        {
            requestMoq = new Mock<HttpRequest>(); 
            loggerMock = new Mock<ILogger>();
            ownerServiceMoq = new Mock<IOwnerService>();

            sut = new testFunc.Owner(ownerServiceMoq.Object);
        }

        [Test]
        public void CreateOwner_WhenRequestDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid Owner input, is NUll or Empty";
            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateTraveller(null, loggerMock!.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result!.Message);

            ownerServiceMoq!.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Never);
            ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Never);
            ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Never);
        }

        [Test]
        public void CreateOwner_WhenRequestBodyDoesntExist_Then_ReturnsUserException()
        {
            var expectedResult = $"Invalid Owner input, is NUll or Empty";
            requestMoq!.Setup(a => a.Body).Returns(value: null!);

            var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateTraveller(requestMoq.Object, loggerMock!.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result!.Message);

            ownerServiceMoq!.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Never);
            ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Never);
            ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Never);
        }

        [Test]
        public void CreateOwner_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = guid.Result;
            var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq!.Setup(a => a.Body).Returns(memoryStream);
           

            ownerServiceMoq!.Setup(a => a.GetOwnerModel(It.IsAny<Stream>())).Returns(Task.FromResult(new Model.Owner() { }));
            ownerServiceMoq.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).Returns(Task.FromResult(false));
            ownerServiceMoq.Setup(a => a.CreateOwner(It.IsAny<Model.Owner>())).Returns(guid);

            var result = sut!.CreateTraveller(requestMoq.Object, loggerMock!.Object);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult, okResult?.Value);

            ownerServiceMoq.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Once);
            ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Once);
            ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Once);
        }

        [Test]
        public async Task CreateOwner_WhenRequestBodyExistsAndOwnerExists_Then_ReturnsSuccessMessageWithValidGuidAsync()
        {
            Task<Guid> guid = Task.FromResult(Guid.NewGuid());
            var expectedResult = await Task.FromResult(guid);
            var json = "{\"Id\":\"00000000-0000-0000-0000-000000000000\",\"FullName\":\"Test User\",\"Email\":\"user@emailprovider.com\",\"Telephone\":\"01234567890\",\"OwnerType\":\"Default\"," +
                "\"CreatedBy\":\"f72591a1-6d8b-e911-a96f-000d3a29b5de\",\"CreatedOn\":\"2023-12-14T10:58:46.3872997+00:00\"," +
                "\"UpdatedBy\":\"f72591a1-6d8b-e911-a96f-000d3a29b5de\",\"UpdatedOn\":\"2023-12-14T10:58:46.3873446+00:00\"," +
                "\"Address\":{\"AddressLine1\":\"27 User Street\",\"AddressLine2\":null,\"TownOrCity\":\"User City\",\"County\":null,\"Postcode\":\"U77 7UU\"}";
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            requestMoq!.Setup(a => a.Body).Returns(memoryStream);

            var ownerModel = new Model.Owner() 
            { 
                Id = guid.Result, 
                Email = "user@emailprovider.com" 
            };

            var ownerEntity = Task.FromResult(new Entities.Owner()
            {
                Id = guid.Result,
                Email = "user@emailprovider.com"
            });


            ownerServiceMoq!.Setup(a => a.GetOwnerModel(It.IsAny<Stream>())).Returns(Task.FromResult(ownerModel));
            ownerServiceMoq.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            ownerServiceMoq.Setup(a => a.CreateOwner(It.IsAny<Model.Owner>())).Returns(guid);
            ownerServiceMoq.Setup(a => a.GetOwnerByEmail(It.IsAny<string>())).Returns(ownerEntity!);

            var result = sut!.CreateTraveller(requestMoq.Object, loggerMock!.Object);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(expectedResult.Result, okResult?.Value);

            ownerServiceMoq.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Once);
            ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Once);
            ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Never);        
        }
    }
}
