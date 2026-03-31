using Model = Defra.PTS.User.Models;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using testFunc = Defra.PTS.User.Functions.Functions.Owner;
using System.Net;
using System.Text;
using Defra.PTS.User.ApiServices.Interface;
using NUnit.Framework;
using Defra.PTS.User.Models.CustomException;
using Newtonsoft.Json;
using Defra.PTS.User.Functions.Tests.Helpers;

namespace Defra.PTS.User.Functions.Tests.Functions.Owner
{
    public class OwnerTest
    {
        private Mock<ILogger<testFunc.Owner>>? loggerMock;
     private Mock<IOwnerService>? ownerServiceMoq;
  testFunc.Owner? sut;

  [SetUp]
        public void SetUp()
  {
        loggerMock = new Mock<ILogger<testFunc.Owner>>();
            ownerServiceMoq = new Mock<IOwnerService>();
          sut = new testFunc.Owner(ownerServiceMoq.Object, loggerMock.Object);
   }

   [Test]
        public void CreateOwner_WhenRequestDoesntExist_Then_ReturnsUserException()
{
          var expectedResult = $"Invalid Owner input, is NUll or Empty";
#pragma warning disable CS8625
    var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateTraveller(null));
#pragma warning restore CS8625

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
   var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

       var result = Assert.ThrowsAsync<UserFunctionException>(() => sut!.CreateTraveller(requestMock));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result!.Message);

ownerServiceMoq!.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Never);
     ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Never);
   ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Never);
        }

[Test]
        public async Task CreateOwner_WhenRequestBodyExists_Then_ReturnsSuccessMessageWithValidGuid()
        {
     var guid = Guid.NewGuid();
  var json = JsonConvert.SerializeObject("{ \"test\" : \"success\" }");
    var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
      var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData(memoryStream);

     ownerServiceMoq!.Setup(a => a.GetOwnerModel(It.IsAny<Stream>())).ReturnsAsync(new Model.Owner() { });
ownerServiceMoq.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).ReturnsAsync(false);
    ownerServiceMoq.Setup(a => a.CreateOwner(It.IsAny<Model.Owner>())).ReturnsAsync(guid);

 var result = await sut!.CreateTraveller(requestMock);

 Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

    ownerServiceMoq.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Once);
  ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Once);
     ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Once);
     }

        [Test]
public async Task CreateOwner_WhenRequestBodyExistsAndOwnerExists_Then_ReturnsSuccessMessageWithValidGuidAsync()
 {
       var guid = Guid.NewGuid();
       var json = "{\"Id\":\"00000000-0000-0000-0000-000000000000\",\"FullName\":\"Test User\",\"Email\":\"user@emailprovider.com\",\"Telephone\":\"01234567890\",\"OwnerType\":\"Default\"," +
                "\"CreatedBy\":\"f72591a1-6d8b-e911-a96f-000d3a29b5de\",\"CreatedOn\":\"2023-12-14T10:58:46.3872997+00:00\"," +
     "\"UpdatedBy\":\"f72591a1-6d8b-e911-a96f-000d3a29b5de\",\"UpdatedOn\":\"2023-12-14T10:58:46.3873446+00:00\"," +
       "\"Address\":{\"AddressLine1\":\"27 User Street\",\"AddressLine2\":null,\"TownOrCity\":\"User City\",\"County\":null,\"Postcode\":\"U77 7UU\"}";
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
     var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData(memoryStream);

            var ownerModel = new Model.Owner() 
   { 
       Id = guid, 
     Email = "user@emailprovider.com" 
            };

            var ownerEntity = new Entities.Owner()
            {
          Id = guid,
      Email = "user@emailprovider.com"
      };

ownerServiceMoq!.Setup(a => a.GetOwnerModel(It.IsAny<Stream>())).ReturnsAsync(ownerModel);
  ownerServiceMoq.Setup(a => a.DoesOwnerExists(It.IsAny<string>())).ReturnsAsync(true);
   ownerServiceMoq.Setup(a => a.CreateOwner(It.IsAny<Model.Owner>())).ReturnsAsync(guid);
     ownerServiceMoq.Setup(a => a.GetOwnerByEmail(It.IsAny<string>())).ReturnsAsync(ownerEntity);

   var result = await sut!.CreateTraveller(requestMock);

    Assert.IsNotNull(result);
 Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

ownerServiceMoq.Verify(a => a.GetOwnerModel(It.IsAny<Stream>()), Times.Once);
      ownerServiceMoq.Verify(a => a.DoesOwnerExists(It.IsAny<string>()), Times.Once);
ownerServiceMoq.Verify(a => a.CreateOwner(It.IsAny<Model.Owner>()), Times.Never);        
        }
    }
}
