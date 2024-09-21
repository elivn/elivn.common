using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kasca.QueuePlug.Queue.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonEnum
    {
        /// <summary>
        /// 
        /// </summary>
        public enum DataPublisherEnum
        {
            /// <summary>
            /// 微软自带队列
            /// </summary>
            [Description("微软自带队列")]
            DefaultQuene=0,
            /// <summary>
            /// RabbitMq
            /// </summary>
            [Description("RabbitMq")]
            RabbitStorageQueue =1
        }
    }
}
