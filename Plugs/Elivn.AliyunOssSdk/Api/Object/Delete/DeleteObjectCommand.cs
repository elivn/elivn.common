using System.Net.Http;
using System.Threading.Tasks;
using Elivn.AliyunOssSdk.Api.Base;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Object.Delete
{
    public class DeleteObjectCommand: BaseObjectCommand<DeleteObjectResult>
    {
        public DeleteObjectCommand(RequestContext requestContext, BucketInfo bucket, string key) : base(requestContext, bucket, key)
        {
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Delete);

            return req;
        }

        public override Task<OssResult<DeleteObjectResult>> ParseResultAsync(HttpResponseMessage response)
        {
            return Task.FromResult(new OssResult<DeleteObjectResult>(new DeleteObjectResult()));
        }
    }
}
