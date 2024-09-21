using System.Collections.Generic;
using System.Net.Http;
using Elivn.AliyunOssSdk.Api.Base;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Object.Copy
{
    public class CopyObjectCommand: BaseObjectCommand<CopyObjectResult>
    {
        public CopyObjectCommand(RequestContext requestContext, BucketInfo targetBucket, string targetObjectKey, BucketInfo srcBucket, string srcObjectKey, IDictionary<string, string> extraHeaders) : base(requestContext, targetBucket, targetObjectKey)
        {
            SrcBucket = srcBucket;
            SrcObjectKey = srcObjectKey;
            ExtraHeaders = extraHeaders;
        }

        public IDictionary<string, string> ExtraHeaders { get; set; }

        public BucketInfo SrcBucket { get; set; }
        public string SrcObjectKey { get; private set; }
        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(Bucket, Key, HttpMethod.Put);
            
            req.Headers.Add("x-oss-copy-source", SrcBucket.MakeResourcePathForSign(SrcObjectKey));
            if (ExtraHeaders != null)
            {
                foreach (var pair in ExtraHeaders)
                {
                    req.Headers.Add(pair.Key, pair.Value);
                }
            }

            return req;
        }
    }
}
