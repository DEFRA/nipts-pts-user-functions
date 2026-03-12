using Defra.PTS.User.ApiServices.Interface;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using testFunc = Defra.PTS.User.Functions.Functions;
using System.Net;
using Defra.PTS.User.Functions.Tests.Helpers;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{
    public class HealthCheckTest
    {
        private Mock<IUserService>? userServiceMoq;
     private Mock<ILogger<testFunc.HealthCheck>>? loggerMock;
        testFunc.HealthCheck? sut;

        [SetUp] 
        public void SetUp()
        {
   loggerMock = new Mock<ILogger<testFunc.HealthCheck>>();
            userServiceMoq = new Mock<IUserService>();
      sut = new testFunc.HealthCheck(userServiceMoq.Object, loggerMock.Object);
 }

        [Test]
     public async Task HealthCheck_WhenTrue_Then_ReturnsServiceAvailable()
        {
            var requestMoq = HttpRequestDataHelper.CreateMockHttpRequestData();
   userServiceMoq!.Setup(a => a.PerformHealthCheckLogic()).Returns(Task.FromResult(true));
        
 var result = await sut!.Run(requestMoq);
        
  Assert.IsNotNull(result);
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
    userServiceMoq.Verify(a => a.PerformHealthCheckLogic(), Times.Once);
    }

  [Test]
        public async Task HealthCheck_WhenFalse_Then_ReturnsServiceUnavailable()
        {
       var requestMoq = HttpRequestDataHelper.CreateMockHttpRequestData();
       userServiceMoq!.Setup(a => a.PerformHealthCheckLogic()).Returns(Task.FromResult(false));
        
      var result = await sut!.Run(requestMoq);
         
      Assert.IsNotNull(result);
    Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
            userServiceMoq.Verify(a => a.PerformHealthCheckLogic(), Times.Once);
        }
    }
}
