using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Defra.PTS.User.Functions.Tests.Helpers
{
    public static class HttpRequestDataHelper
    {
        /// <summary>
        /// Creates a FakeHttpRequestData that works with the real CreateResponse extension method
        /// </summary>
        public static FakeHttpRequestData CreateMockHttpRequestData(Stream? body = null, FunctionContext? context = null)
        {
            var mockContext = context ?? CreateMockFunctionContext();
            return new FakeHttpRequestData(mockContext, body: body);
        }

        public static FunctionContext CreateMockFunctionContext()
        {
            var services = new ServiceCollection();
            services.AddOptions<WorkerOptions>().Configure(options =>
                {
                    options.Serializer = new JsonObjectSerializer(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                });
            
            var serviceProvider = services.BuildServiceProvider();

            // Use Mock<FunctionContext> to avoid needing to implement all abstract members
            var mockContext = new Mock<FunctionContext>();
            mockContext.Setup(c => c.InstanceServices).Returns(serviceProvider);
            mockContext.Setup(c => c.InvocationId).Returns(Guid.NewGuid().ToString());
            mockContext.Setup(c => c.FunctionId).Returns(Guid.NewGuid().ToString());
            mockContext.Setup(c => c.Features).Returns(Mock.Of<IInvocationFeatures>());
            mockContext.Setup(c => c.Items).Returns(new Dictionary<object, object>());
        
             return mockContext.Object;
         }
    }
}
