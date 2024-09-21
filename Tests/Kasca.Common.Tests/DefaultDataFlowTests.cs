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

namespace Kasca.Common.Tests
{
    [TestClass]
    public class DefaultDataFlowTests : BaseTests
    {

        public class MsgPoper : IDataSubscriber<MsgData>
        {
            public async Task<bool> Subscribe(MsgData data)
            {
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            }
        }
        private static readonly IDataPublisher _normalFlowPublisher = DataFlowFactory.RegisterFlow("normalFlow", new MsgPoper());

        [TestMethod]
        public async Task DataStackTest()
        {
            var pushRes = await _normalFlowPublisher.Publish("normalFlow", new MsgData() { name = "test" });
            Assert.IsTrue(pushRes);

            await Task.Delay(2000);
        }




        private static readonly IDataPublisher _delegateFlowpusher = DataFlowFactory.RegisterFlow<MsgData>("delegateFlow", async (data) =>
        {
            //  执行订阅业务, 这里是委托方法的形式，也可以传入继承 IDataSubscriber<MsgData> 接口的实例
            await Task.Delay(1000);
            Assert.IsTrue(data.name == "test");
            return true;
        });

        [TestMethod]
        public async Task DataStackFuncTest()
        {
            var pushRes = await _delegateFlowpusher.Publish("delegateFlow", new MsgData() { name = "test" });
            Assert.IsTrue(pushRes);
            await Task.Delay(2000);
        }




        private const string msgPSKey = "P-S-Key";
        private static readonly IDataPublisher _publisher;

        static DefaultDataFlowTests()
        {
            _publisher = DataFlowFactory.CreatePublisher(new DataPublisherOption()
            {
                //  使用默认实现时，会创建名称为 NewSource 的队列 
                source_name = "NewSourceQueue"
            });

            DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                // DoSomething(data)   订阅功能实现
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true; // 消费成功
            });

            DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                // DoSomething(data)   订阅功能实现
                await Task.Delay(1000);
                Assert.IsTrue(data.name == "test");
                return true;
            });
        }

        [TestMethod]
        public async Task DataPublisherAndMultiSubscriberTest()
        {
            var pushRes = await _publisher.Publish(msgPSKey, new MsgData() { name = "test" });
            Assert.IsTrue(pushRes);

            await Task.Delay(2000);// 延时观察异常订阅者处理情况
        }
    }

}
