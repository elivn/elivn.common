using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;
using Kasca.QueuePlug.Queue;
using Kasca.QueuePlug.Queue.rabbit;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class DataFlowTests : BaseTests
    {

        private static readonly IDataPublisher _customMsgPublisher;
        static DataFlowTests()
        {
            //  正常应该在程序全局入口处注册，这里为了测试
          //  DataFlowManager.PublisherProvider = new RabbitStorageProvider();

            //_customMsgPublisher = DataFlowFactory.RegisterFlow<MsgData>("testrabbitkey", async (data) =>
            //{
            //    //  执行订阅业务, 这里是委托方法的形式，也可以传入继承 IDataSubscriber<MsgData> 接口的实例
            //    await Task.Delay(1000);
            //    Assert.IsTrue(data.name == "testrabbit");
            //    return true;
            //}, new DataPublisherOption { source_name = "RabbitStorageQueue" });


            RabbitSubscriber subscriber = new QueuePlug.Queue.rabbit.RabbitSubscriber();
            subscriber.RegisterSubscriber("testrabbitkey", DataReceviedMessage);


        }

        /// <summary>
        /// RabbmitMQ消费者消费
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void DataReceviedMessage(object obj, BasicDeliverEventArgs e)
        {
            string InfoMessage = string.Empty;

            var body = e.Body.ToArray();
            string Message = Encoding.UTF8.GetString(body);
            var input = JsonConvert.DeserializeObject<MsgData>(Message);
            Assert.IsTrue(input.name == "testrabbit");
        }

        private static readonly IDataPublisher _delegateFlowpusher;

        [TestMethod]
        public async Task RabbitStorageFuncTest()
        {
            RabbitDataPublisher _delegateFlowpusher = new RabbitDataPublisher();
            var pushRes = await _delegateFlowpusher.Publish("testrabbitkey", new MsgData() { name = "testrabbit" });

           // var pushRes = await RabbitDataPublisher _delegateFlowpusher.Publish("testrabbitkey", new MsgData() { name = "testrabbit" });
            Assert.IsTrue(pushRes);
            await Task.Delay(2000);
        }
    }

    public class MsgData
    {
        public string name { get; set; }
    }
}
