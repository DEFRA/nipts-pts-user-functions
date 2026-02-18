using Defra.PTS.User.ApiServices.Interface;
using Defra.PTS.User.Models.CustomException;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Text;
using testFunc = Defra.PTS.User.Functions.Functions.Address;
using Defra.PTS.User.Functions.Tests.Helpers;

namespace Defra.PTS.User.Functions.Tests.Functions.Address
{
    [TestFixture]
    public class AddressTest
    {
      private Mock<IAddressService> _mockAddressService = new();
      private Mock<ILogger<testFunc.Address>> _mockLogger = new();
        testFunc.Address? _sut;

[SetUp]
    public void Setup()
        {
            _mockAddressService = new Mock<IAddressService>();
            _mockLogger = new Mock<ILogger<testFunc.Address>>();
     _sut = new testFunc.Address(_mockAddressService.Object, _mockLogger.Object);
        }

      [TearDown]
  public void Teardown()
        {
            _mockAddressService.Reset();
_mockLogger.Reset();
}

        [Test]
        public async Task CreateAddress()
        {
 var json = JsonConvert.SerializeObject(null);
     var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
     var addressId = Guid.NewGuid();
      var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData(memoryStream);

      _mockAddressService.Setup(x => x.GetAddressModel(It.IsAny<Stream>()))
    .ReturnsAsync(new Models.Address());
            _mockAddressService.Setup(x => x.CreateAddress(It.IsAny<Models.Address>()))
.ReturnsAsync(addressId);

            var result = await _sut!.CreateAddress(requestMock.Object);

         Assert.IsNotNull(result);
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

     [Test]
      public void CreateAddress_Throw_Exception()
    {
      var expectedMessage = "Invalid Address input, is NUll or Empty";
      var requestMock = HttpRequestDataHelper.CreateMockHttpRequestData();

    var result = Assert.ThrowsAsync<AddressFunctionException>(() => _sut!.CreateAddress(requestMock.Object));

            Assert.IsNotNull(result);
          Assert.AreEqual(expectedMessage, result!.Message);
        }
    }
}
