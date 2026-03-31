using System.Net;
using Defra.PTS.User.Functions.Tests.Helpers;
using Microsoft.Azure.Functions.Worker.Http;
using NUnit.Framework;

namespace Defra.PTS.User.Functions.Tests
{
    [TestFixture]
    public class FakeHttpDataTests
    {
        [Test]
        public void FakeHttpResponseData_StatusCodeCanBeSet()
        {
            var context = HttpRequestDataHelper.CreateMockFunctionContext();
            var response = new FakeHttpResponseData(context, HttpStatusCode.OK);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            response.StatusCode = HttpStatusCode.BadRequest;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void FakeHttpRequestData_CreateResponse_ReturnsCorrectStatusCode()
        {
            var context = HttpRequestDataHelper.CreateMockFunctionContext();
            var request = new FakeHttpRequestData(context);

            var response = request.CreateResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            response.StatusCode = HttpStatusCode.BadRequest;
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task CreateResponseExtension_WithStatusCode_SetsCorrectly()
        {
            var request = HttpRequestDataHelper.CreateMockHttpRequestData();

            // Use the real extension method pattern - set status code AFTER writing content
            var badResponse = request.CreateResponse();
            await badResponse.WriteAsJsonAsync("test");
            badResponse.StatusCode = HttpStatusCode.BadRequest;

            Assert.AreEqual(HttpStatusCode.BadRequest, badResponse.StatusCode);
        }
    }
}
