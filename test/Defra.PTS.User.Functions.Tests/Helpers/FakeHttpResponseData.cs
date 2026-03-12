using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Defra.PTS.User.Functions.Tests.Helpers
{
    /// <summary>
    /// A fake implementation of HttpResponseData for testing purposes
    /// </summary>
    public class FakeHttpResponseData : HttpResponseData
    {
        private HttpStatusCode _statusCode;

        public FakeHttpResponseData(FunctionContext functionContext, HttpStatusCode statusCode = HttpStatusCode.OK) 
            : base(functionContext)
        {
            _statusCode = statusCode;
            Headers = new HttpHeadersCollection();
            Body = new MemoryStream();
        }

        public override HttpStatusCode StatusCode 
        { 
            get => _statusCode; 
            set => _statusCode = value; 
        }
        public override HttpHeadersCollection Headers { get; set; }
        public override Stream Body { get; set; }
        public override HttpCookies Cookies { get; } = null!;
    }
}
