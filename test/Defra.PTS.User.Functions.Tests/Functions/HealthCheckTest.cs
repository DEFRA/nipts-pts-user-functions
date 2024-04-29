using model = Defra.PTS.User.Models;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using testFunc = Defra.PTS.User.Functions.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Defra.PTS.User.Models;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{
    public class HealthCheckTest
    {
        private Mock<HttpRequest> requestMoq;
        private Mock<ILogger> loggerMock;
        private Mock<IUserService> userServiceMoq;
        testFunc.HealthCheck sut;

        [SetUp] 
        public void SetUp()
        {
            requestMoq = new Mock<HttpRequest>();
            loggerMock = new Mock<ILogger>();
            userServiceMoq = new Mock<IUserService>();

            sut = new testFunc.HealthCheck(userServiceMoq.Object);
        }

        [Test]
        public void HealthCheck_WhenTrue_Then_ReturnsServiceAvailable()
        {            
            userServiceMoq.Setup(a => a.PerformHealthCheckLogic()).Returns(Task.FromResult(true));
            var result = sut.Run(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as OkResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(200, okResult?.StatusCode);
            userServiceMoq.Verify(a => a.PerformHealthCheckLogic(), Times.Once);
        }

        [Test]
        public void HealthCheck_WhenFalse_Then_ReturnsServiceUnavailable()
        {
            userServiceMoq.Setup(a => a.PerformHealthCheckLogic()).Returns(Task.FromResult(false));
            var result = sut.Run(requestMoq.Object, loggerMock.Object);
            var okResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(503, okResult?.StatusCode);
            userServiceMoq.Verify(a => a.PerformHealthCheckLogic(), Times.Once);
        }
    }
}
