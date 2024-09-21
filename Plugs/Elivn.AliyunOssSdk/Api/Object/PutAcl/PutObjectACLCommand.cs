using System.Net.Http;
using System.Threading.Tasks;
using Elivn.AliyunOssSdk.Api.Base;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Object.PutAcl
{
    public class PutObjectACLCommand:BaseObjectCommand<EmptyResult>
    {
        public string AclType { get; set; }

        public PutObjectACLCommand(RequestContext requestContext, BucketInfo bucket, string key, string aclType) : base(requestContext, bucket, key)
        {
            AclType = aclType;
        }

        public override ServiceRequest BuildRequest()
        {
           var req = new ServiceRequest(Bucket, Key, HttpMethod.Put);

            req.Parameters.Add("acl", "");

            req.Headers.Add("x-oss-object-acl", AclType);

            return req;
        }

        public override Task<OssResult<EmptyResult>> ParseResultAsync(HttpResponseMessage response)
        {
            return Task.FromResult(new OssResult<EmptyResult>(new EmptyResult()));
        }
    }
}
