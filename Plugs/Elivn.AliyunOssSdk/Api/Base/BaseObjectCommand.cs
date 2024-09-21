using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;

namespace Elivn.AliyunOssSdk.Api.Base
{
    /// <summary>
    /// 对某个对象进行的操作的命令基类，主要是保存了Bucket信息和key
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BaseObjectCommand<TResult> : BaseOssCommand<TResult>
    {
        /// <summary>
        /// 对象所处的Bucket
        /// </summary>
        public BucketInfo Bucket { get; set; }

        /// <summary>
        /// 目标对象的Key
        /// </summary>
        public string Key { get; set; }

        public BaseObjectCommand(RequestContext requestContext,
            BucketInfo bucket,
            string key) : base(requestContext)
        {
            Bucket = bucket;
            Key = key;
        }
    }
}
