using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Functions.Functions.User;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net;
using Defra.PTS.User.Functions.Tests.Helpers;

namespace Defra.PTS.User.Functions.Tests.Functions.User
{
    public class IsUserSuspendedTest
    {
        private Mock<IUserService> _mockUserService = new();
        private Mock<ILogger<IsUserSuspended>> _mockLogger = new();
        private IsUserSuspended? _sut;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<IsUserSuspended>>();
            _sut = new IsUserSuspended(_mockUserService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockLogger.Reset();
            _mockUserService.Reset();
        }

        [Test]
        public async Task IsUserSuspended_InvalidContactId_ReturnsBadRequest()
        {
            var contactId = "Invalid Id";
            var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

            var result = await _sut!.Run(requestMock, contactId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            _mockUserService.Verify(s => s.IsUserSuspended(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task IsUserSuspended_SuspendedUser_ReturnsOk()
        {
            var contactId = Guid.NewGuid();
            _mockUserService.Setup(s => s.IsUserSuspended(contactId)).ReturnsAsync(true);
            var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

            var result = await _sut!.Run(requestMock, contactId.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _mockUserService.Verify(s => s.IsUserSuspended(contactId), Times.Once);
        }

        [Test]
        public async Task IsUserSuspended_NotSuspendedUser_ReturnsOk()
        {
            var contactId = Guid.NewGuid();
            _mockUserService.Setup(s => s.IsUserSuspended(contactId)).ReturnsAsync(false);
            var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

            var result = await _sut!.Run(requestMock, contactId.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _mockUserService.Verify(s => s.IsUserSuspended(contactId), Times.Once);
        }
    }
}
