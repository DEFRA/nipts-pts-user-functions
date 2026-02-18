using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Functions.Functions.User;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net;
using Defra.PTS.User.Functions.Tests.Helpers;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{    
    public class GetUserDetailTest
    {
    private Mock<IUserService> _mockUserService = new();
private Mock<ILogger<GetUserDetail>> _mockLogger = new();
     private GetUserDetail? _sut;

      [SetUp]
  public void Setup()
        {
      _mockUserService = new Mock<IUserService>();
 _mockLogger = new Mock<ILogger<GetUserDetail>>();
            _sut = new GetUserDetail(_mockUserService.Object, _mockLogger.Object);
 }

        [TearDown]
        public void TearDown()
    {
      _mockLogger.Reset();
  _mockUserService.Reset();
        }

        [Test]
        public async Task GetUserDetail_InvalidUserId()
        {
     var userId = "Invalid Id";
            var expectedValue = "You must provide a valid value for userId";
      var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

          var result = await _sut!.Run(requestMock.Object, userId);
   
Assert.IsNotNull(result);
    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

        [Test]
        public async Task GetUserDetail()
        {
         var userId = Guid.NewGuid().ToString();
      var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

            var result = await _sut!.Run(requestMock.Object, userId);
            
            Assert.IsNotNull(result);
   Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }
    }
}