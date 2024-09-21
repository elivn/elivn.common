using System.Net.Http;
using Elivn.AliyunOssSdk.Api.Base;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Object.GetAcl
{
    public class GetObjectAclCommand:BaseObjectCommand<GetObjectAclResult>
    {
        public GetObjectAclCommand(RequestContext requestContext, BucketInfo bucket, string key) : base(requestContext, bucket, key)
        {
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Get);

            req.Parameters.Add("acl", "");

            return req;
        }
    }
}
