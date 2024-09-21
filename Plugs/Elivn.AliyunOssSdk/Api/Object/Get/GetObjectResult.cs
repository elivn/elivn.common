using System.Net.Http;
using System.Net.Http.Headers;
using Elivn.AliyunOssSdk.Entites;

namespace Elivn.AliyunOssSdk.Api.Object.Get
{
    public class GetObjectResult
    {
        public HttpResponseHeaders Headers { get; set; }

        public HttpContent Content { get; set; }

        public ObjectMetadata Metadata { get; set; }
    }
}
