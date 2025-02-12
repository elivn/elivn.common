﻿using System.Net.Http;
using Elivn.AliyunOssSdk.Api.Base;
using Elivn.AliyunOssSdk.Api.Common.Consts;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Bucket.List
{
    public class ListBucketsCommand: BaseOssCommand<ListBucketsResult>
    {
        private ListBucketsRequest _request;
        private string _region;

        public ListBucketsCommand(RequestContext requestContext, string region, ListBucketsRequest request) : base(requestContext)
        {
            _request = request;
            _region = region;
        }

        public override ServiceRequest BuildRequest()
        {
            var req = new ServiceRequest(BucketInfo.CreateByRegion(_region, ""), "", HttpMethod.Get);

            req.AddParameter(RequestParameters.PREFIX, _request.Prefix);
            req.AddParameter(RequestParameters.MARKER, _request.Marker);
            req.AddParameter(RequestParameters.MAX_KEYS, _request.MaxKeys?.ToString());

            return req;
        }
        
    }
}
