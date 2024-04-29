using Castle.Core.Logging;
using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Functions.Functions.User;
using Defra.PTS.User.Models.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{    
    public class GetUserDetailTest
    {
        private readonly Mock<IUserService> _mockUserService = new();
        private readonly Mock<ILogger<GetUserDetail>> _mockLogger = new();
        private readonly Mock<HttpRequest> _mockRequest = new();
        private GetUserDetail? _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GetUserDetail(_mockUserService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockLogger.Reset();
            _mockUserService.Reset();
            _mockRequest.Reset();
        }

        [Test]
        public async Task GetUserDetail_InvalidUserId()
        {
            var userId = "Invalid Id";
            var expectedValue = "You must provide a valid value for userId";
            

            var result = await _sut!.Run(_mockRequest.Object, userId);
            var badResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);
            Assert.AreEqual(400, badResult?.StatusCode);
            Assert.AreEqual(expectedValue, badResult?.Value);
        }

        [Test]
        public async Task GetUserDetail()
        {
            var userId = Guid.NewGuid().ToString();           

            var result = await _sut!.Run(_mockRequest.Object, userId);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);            
        }
    }
}