using Kasca.Common.ComUtils;
using Kasca.Common.Plugs.LogPlug;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kasca.QueuePlug.Queue.rabbit
{
    public class RabbitDataPublisher : IDataPublisher
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

        private static IModel Channel = null;

        #region 单例创建IModel
        private static object lockObject = new Object();
        /// <summary>
        /// 单例获取IModel
        /// </summary>
        /// <returns></returns>
        private static IModel GetInstance()
        {
            if (Channel == null)
            {
                lock (lockObject)
                {
                    factory = new ConnectionFactory();
                    factory.HostName = RabbitMQHost;
                    factory.Port = RabbitMQPort;
                    factory.UserName = RabbitMQUserName;
                    factory.Password = RabbitMQPassword;
                    connection = factory.CreateConnection();
                    Channel = connection.CreateModel();
                }
            }
            return Channel;
        }
        #endregion


        private readonly DataPublisherOption _option;


        /// <summary>
        ///   发布数据
        /// </summary>
        /// <param name="dataTypeKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> Publish<TData>(string dataTypeKey, TData data)
        {
            return Task.FromResult(ProduceQueueMessage(dataTypeKey, JsonConvert.SerializeObject(data)));
        }

        /// <summary>
        /// RabbitMQ生产消息
        /// </summary>
        /// <param name="QueueKey"></param>
        /// <param name="Json"></param>
        public static bool ProduceQueueMessage(string QueueKey, string Json)
        {
            try
            {
                if (Channel == null)
                {
                    GetInstance();
                }
                Channel.ConfirmSelect();
                Channel.BasicAcks += BasicAcks;
                Channel.BasicNacks += BasicNacks;
                Channel.QueueDeclare(QueueKey, false, false, false, null);
                var properties = Channel.CreateBasicProperties();
                properties.DeliveryMode = 1;
                properties.Persistent = true;
                Channel.BasicPublish("", QueueKey, properties, Encoding.UTF8.GetBytes(Json));
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Error($"RabbitMQ生产消息异常,异常信息:{ex.ToJson()}");
                return false;
            }
        }


        private static void BasicAcks(object obj, BasicAckEventArgs e)
        {

        }
        private static void BasicNacks(object obj, BasicNackEventArgs e)
        {

        }
    }
}
