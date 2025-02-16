﻿using Kasca.QueuePlug.Queue.Inter.Queue;
using System.Threading.Tasks;

namespace Kasca.QueuePlug.Queue
{
    /// <summary>
    /// 消息流核心部件管理者
    /// </summary>
    public static class DataFlowManager
    {
        /// <summary>
        /// 自定义 数据流发布（存储）实现的 提供者
        /// </summary>
        public static IDataPublisherProvider PublisherProvider { get; set; }

        /// <summary>
        ///  通过自定义消息触发机制通知订阅者
        ///     调用时请做异常拦截，防止脏数据导致 msgData 类型错误
        /// </summary>
        /// <param name="msgDataKey"></param>
        /// <param name="msgData">消息内容，自定义触发时，请注意和注册订阅者的消费数据类型转换安全</param>
        /// <returns></returns>
        public static Task<bool> NotifySubscriber(string msgDataKey, object msgData)
        {
            return _subscriberHandler.NotifySubscriber(msgDataKey, msgData);
        }

        private readonly static InterSubscriberHandler _subscriberHandler = new InterSubscriberHandler();
        //  注册订阅者
        internal static bool RegisterSubscriber<TData>(string msgFlowKey, IDataSubscriber<TData> subscriber, DataPublisherOption option = null)
        {
            if (option?.source_name == "RabbitStorageQueue")
            {
               // _subscriberHandler.RegisterSubscriber(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber));
                return true;
            }
            else
            {
                _subscriberHandler.RegisterSubscriber(msgFlowKey, new InterDataSubscriberWrap<TData>(subscriber));
                return true;
            }
        }
    }
}