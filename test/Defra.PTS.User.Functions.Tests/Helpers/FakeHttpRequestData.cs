using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Defra.PTS.User.Functions.Tests.Helpers
{
    /// <summary>
    /// A fake implementation of HttpRequestData for testing purposes
    /// </summary>
    public class FakeHttpRequestData : HttpRequestData
    {
        private readonly Stream? _body;

        public FakeHttpRequestData(FunctionContext functionContext, Uri? url = null, Stream? body = null) 
   : base(functionContext)
        {
            Url = url ?? new Uri("https://localhost");
_body = body; // Store the actual body, can be null
      Headers = new HttpHeadersCollection();
Cookies = new List<IHttpCookie>();
        Identities = new List<ClaimsIdentity>();
        }

      public override Stream Body => _body!; // Return the actual body value, which can be null
      public override HttpHeadersCollection Headers { get; }
        public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
public override Uri Url { get; }
        public override IEnumerable<ClaimsIdentity> Identities { get; }
   public override string Method { get; } = "POST";

     public override HttpResponseData CreateResponse()
        {
    return new FakeHttpResponseData(FunctionContext);
        }
    }
}
