using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace Defra.PTS.User.Functions.Tests.Helpers
{
    public static class HttpRequestDataHelper
    {
        public static Mock<HttpRequestData> CreateMockHttpRequestData(Stream? body = null, FunctionContext? context = null)
        {
            var mockContext = context ?? CreateMockFunctionContext();
var mockRequest = new Mock<HttpRequestData>(mockContext);

  if (body != null)
   {
     mockRequest.Setup(r => r.Body).Returns(body);
         }

            mockRequest.Setup(r => r.CreateResponse()).Returns(() =>
     {
        var response = new Mock<HttpResponseData>(mockContext);
            response.SetupProperty(r => r.StatusCode);
        response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
   response.Setup(r => r.Body).Returns(new MemoryStream());
                return response.Object;
   });

        return mockRequest;
        }

        public static FunctionContext CreateMockFunctionContext()
        {
        var mockContext = new Mock<FunctionContext>();
      var serviceProvider = new Mock<IServiceProvider>();
      mockContext.Setup(c => c.InstanceServices).Returns(serviceProvider.Object);
            return mockContext.Object;
        }

    public static Mock<HttpResponseData> CreateMockHttpResponseData(FunctionContext? context = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
          var mockContext = context ?? CreateMockFunctionContext();
var mockResponse = new Mock<HttpResponseData>(mockContext);
            
          mockResponse.SetupProperty(r => r.StatusCode, statusCode);
          mockResponse.SetupProperty(r => r.Headers, new HttpHeadersCollection());
       mockResponse.Setup(r => r.Body).Returns(new MemoryStream());
            
      return mockResponse;
        }
    }
}
