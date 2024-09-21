using System;
using System.Collections.Generic;
using System.Text;

namespace Kasca.QueuePlug.Queue.rabbit
{
    /// <summary>
    /// rabbit消息存储适配器
    /// </summary>
    public class RabbitStorageProvider : IDataPublisherProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public IDataPublisher CreatePublisher(DataPublisherOption option)
        {
            return option.source_name == "RabbitStorageQueue" ? new RabbitDataPublisher() : null;
        }
    }
}
