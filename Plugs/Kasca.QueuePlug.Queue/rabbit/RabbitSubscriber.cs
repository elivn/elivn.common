using Kasca.Common.ComUtils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kasca.QueuePlug.Queue.rabbit
{
    public class RabbitSubscriber
    {
        #region RabbitMQ相关配置
        /// <summary>
        /// RabbitMQ服务器
        /// </summary>
        private static readonly string RabbitMQHost = ConfigUtil.GetSection("RabbitMQ:Host").Value;

        /// <summary>
        /// RabbitMQ服务器
        /// </summary>
        private static readonly int RabbitMQPort = Convert.ToInt32(ConfigUtil.GetSection("RabbitMQ:Port").Value);

        /// <summary>
        /// RabbitMQ登录账号
        /// </summary>
        private static readonly string RabbitMQUserName = ConfigUtil.GetSection("RabbitMQ:UserName").Value;

        /// <summary>
        /// RabbitMQ登录密码
        /// </summary>
        private static readonly string RabbitMQPassword = ConfigUtil.GetSection("RabbitMQ:Password").Value;
        #endregion

        private static ConnectionFactory factory = null;

        private static IConnection connection = null;



        #region 单例创建IModel
        private static object lockObject = new Object();
        /// <summary>
        /// 单例获取IModel
        /// </summary>
        /// <returns></returns>
        public static IConnection GetInstance()
        {
            if (connection == null)
            {
                lock (lockObject)
                {
                    factory = new ConnectionFactory();
                    factory.HostName = RabbitMQHost;
                    factory.Port = RabbitMQPort;
                    factory.UserName = RabbitMQUserName;
                    factory.Password = RabbitMQPassword;
                    connection = factory.CreateConnection();
                }
            }
            return connection;
        }
        #endregion

        //  private readonly Func<TData, Task<bool>> _subscriber;

        public RabbitSubscriber()
        {
            // _subscriber = subscribeFunc ?? throw new ArgumentNullException(nameof(subscribeFunc), "订阅者方法不能为空！");
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="msgDataTypeKey"></param>
        /// <param name="subrabbit"></param>
        /// <param name="queueDeclareOption"></param>
        /// <param name="basicQos"></param>
        /// <returns></returns>
        public bool RegisterSubscriber(string msgDataTypeKey, EventHandler<BasicDeliverEventArgs> subrabbit, QueueDeclareOption queueDeclareOption=null, BasicQosOption basicQos=null)
        {
            connection = GetInstance();
            var Channel = connection.CreateModel();
            if (queueDeclareOption == null)
                queueDeclareOption = new QueueDeclareOption();
            if (basicQos == null)
                basicQos = new BasicQosOption();
            Channel.BasicQos(basicQos.prefetchSize, basicQos.prefetchCount, basicQos.global);
            Channel.QueueDeclare(msgDataTypeKey, queueDeclareOption.durable , queueDeclareOption.exclusive, queueDeclareOption.autoDelete, queueDeclareOption.arguments);
            var consumerData = new EventingBasicConsumer(Channel);
            Channel.BasicConsume(msgDataTypeKey, true, consumerData);
            consumerData.Received += subrabbit;
            return true;
        }
    }

    //
    // 摘要:
    //     Declares a queue. See the Queues guide to learn more.
    //
    // 参数:
    //   durable:
    //     Should this queue will survive a broker restart?
    //
    //   exclusive:
    //     Should this queue use be limited to its declaring connection? Such a queue will
    //     be deleted when its declaring connection closes.
    //
    //   autoDelete:
    //     Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
    //
    //   arguments:
    //     Optional; additional queue arguments, e.g. "x-queue-type"
    public class QueueDeclareOption
    {
        /// <summary>
        /// Should this queue will survive a broker restart?
        /// </summary>
        public bool durable { get; set; } = false;
        /// <summary>
        ///  Should this queue use be limited to its declaring connection? Such a queue will
        ///  be deleted when its declaring connection closes.
        /// </summary>
        public bool exclusive { get; set; } = false;
        /// <summary>
        ///  Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
        /// </summary>
        public bool autoDelete { get; set; } = false;
        /// <summary>
        ///  Optional; additional queue arguments, e.g. "x-queue-type"
        /// </summary>
        public IDictionary<string, object> arguments { get; set; }
    }

    /// <summary>
    /// 如：BasicQos(0, 1, false)设置broker每次只推送队列里面的一条消息到消费者,只有在确认这条消息"成功消费"后,才会继续推送 
    /// </summary>
    public class BasicQosOption
    {
        public uint prefetchSize { get; set; } = 0;

        public ushort prefetchCount { get; set; } = 1;

        public bool global { get; set; } = false;
    }

}