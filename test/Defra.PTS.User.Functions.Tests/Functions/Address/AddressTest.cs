using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;
using testFunc = Defra.PTS.User.Functions.Functions.Address;

namespace Defra.PTS.User.Functions.Tests.Functions.Address
{
    [TestFixture]
    public class AddressTest
    {
        private readonly Mock<HttpRequest> _requestMoq = new();
        private readonly Mock<ILogger<testFunc.Address>> _mockLogger = new();
        private readonly Mock<IAddressService> _mockAddressService = new();
        testFunc.Address? _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new testFunc.Address(_mockAddressService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void Teardown()
        {
            _requestMoq.Reset();
            _mockLogger.Reset();
            _mockAddressService.Reset();
        }

        [Test]
        public async Task CreateAddress()
        {
            var json = JsonConvert.SerializeObject(null);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var addressId = Guid.NewGuid();

            _requestMoq.Setup(a => a.Body).Returns(memoryStream);
            _mockAddressService.Setup(x => x.GetAddressModel(It.IsAny<Stream>()))
                .ReturnsAsync(new Models.Address());
            _mockAddressService.Setup(x => x.CreateAddress(It.IsAny<Models.Address>()))
                .ReturnsAsync(addressId);

            var result = await _sut!.CreateAddress(_requestMoq.Object);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult?.StatusCode);
            Assert.AreEqual(addressId, okResult?.Value);
        }

        [Test]
        public void CreateAddress_Throw_Exception()
        {                        
            var expectedMessage = "Invalid Address input, is NUll or Empty";

            var result = Assert.ThrowsAsync<AddressFunctionException>(() => _sut!.CreateAddress(_requestMoq.Object));

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result!.Message);
        }
    }
}
