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
using Kasca.QueuePlug.Queue.Event;
using System;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class FlowEventTests
    {
        [TestMethod]
        public async Task DataStackTest()
        {
            var o = new FlowEventOption()
            {
                event_msg_key = "Test_flow_event_msg",
                flow_retry_times = 4, // ������Ϣ�����Դ���
                func_retry_times = 1  //  ��ǰִ�з����ڲ�ֱ�Ӵ���ѭ�����Դ���
            };
            var flowProcessor = new FlowEventProcessor<TestCount, TestCount>(new TestEvent(), o);

            var countPara = new TestCount() { count = 0 };

            var countRes = await flowProcessor.Process(countPara);
            Assert.IsNull(countRes); // �״��׳��쳣�����ط��ؿ�

            await Task.Delay(5000);  // �첽��Ϣ�������ѻ���

            // ��ִ�д��� = (flow_retry_times+1)*(func_retry_times+1) = (4+1)*(1+1) = 10
            Assert.IsTrue(countPara.count == 10);// Ĭ����Ϣ����ʵ�����ڴ漶�����ò���
        }

    }

    // ����ִ���¼�
    public class TestEvent : IFlowEvent<TestCount, TestCount>
    {
        public Task<TestCount> Execute(TestCount input)
        {
            input.count++;
            if (input.count < 10)
            {
                throw new ArgumentException("С�ڵ�ǰҪ�������");
            }
            return Task.FromResult(input);
        }

        public Task Failed(TestCount input)
        {
            Console.WriteLine(input.count);
            return Task.CompletedTask;
        }
    }
    public class TestCount
    {
        public int count { get; set; }
    }
}
