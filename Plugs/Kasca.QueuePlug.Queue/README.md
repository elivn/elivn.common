
## 消息流使用
##RabbitMq使用
使用示例：
  a. 消息的发布订阅独立调用示例  
```csharp

	// 订阅者实现
	RabbitSubscriber subscriber = new QueuePlug.Queue.rabbit.RabbitSubscriber();
    subscriber.RegisterSubscriber("testrabbitkey", DataReceviedMessage);
    ///事件
    private static void DataReceviedMessage(object obj, BasicDeliverEventArgs e)
        {
            string InfoMessage = string.Empty;

            var body = e.Body.ToArray();
            string Message = Encoding.UTF8.GetString(body);
            var input = JsonConvert.DeserializeObject<MsgData>(Message);
            Assert.IsTrue(input.name == "testrabbit");
        }

	//	业务方法中发布消息
	 RabbitDataPublisher _delegateFlowpusher = new RabbitDataPublisher();
     var pushRes = await _delegateFlowpusher.Publish(msgPSKey, new MsgData() { name = "testrabbit" });

 b.自定义
```csharp
    var connetction = RabbitSubscriber.GetInstance();
    var channel = connection.CreateModel();
    //后续针对channel自定义操作，比如设置各种自定义参数 
##微软Dataflow使用
使用示例：  
  a. 消息的发布订阅独立调用示例  
```csharp
	// 全局初始化，注入订阅者实现
	const string msgPSKey = "Publisher-Subscriber-MsgKey";
	DataFlowFactory.RegisterSubscriber<MsgData>(msgPSKey, async (data) =>
            {
                // 当前通过注入消费的委托方法，也可通过接口实现
                // DoSomething(data);
                return true;
            });

	//	获取发布者接口
	private static readonly IDataPublisher publisher = DataFlowFactory.CreatePublisher(); 

	//  业务方法中发布消息
	await publisher.Publish(msgPSKey,new MsgData() {name = "test"});
```
  b.  消息的流式调用示例  
```csharp
	// 直接注册消费实现并获取消息发布接口
	private static readonly IDataPublisher _delegateFlowpusher = 
        DataFlowFactory.RegisterFlow<MsgData>("delegate_flow",async (data) =>
            {
                // 当前通过注入消费的委托方法，也可通过接口实现
                // DoSomething(data);
                return true;
            });

	// 业务方法中发布消息
    await _delegateFlowpusher.Publish("normal_flow",new MsgData() {name = "test"});
```
如上，只需要获取发布者，并注入消费实现，即可完成整个消息的异步消费处理，同一个消息key可以注册多个消费实现，当有消息进入消费时，会并发处理。


## 回流（重复执行）事件处理器
在有些比较重要的业务处理中，如果发生异常（如网络超时）等操作，会要求过段时间后重复执行进行错误补偿，借助OSS.DataFlow 类库的消息存储和转发接口，提供了  FlowEventProcessor<TIn, TOut>   的事件处理器，在事件处理器内部完成了异常拦截重试的封装。  
具体的过程就是当异常发生时，处理器通过将入参包装（FlowEventInput<TIn>）通过消息流保存，具体的重试触发实现间隔，由开发者根据 MsgKey和参数 FlowEventInput 自行定制实现即可。  

一. 使用示例：  
```csharp
 	// 具体执行事件
    public class TestEvent:IFlowEvent<TestCount, TestCount>
    {
        public Task<TestCount> Execute(TestCount input)
        {
            input.count++;
            if (input.count < 10)
            {
                throw new ArgumentException("小于当前要求的条件");
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

	//  单元测试方法
   	[TestMethod]
   	public async Task DataStackTest()
   	{
       var o = new FlowEventOption()
       {
           event_msg_key = "Test_flow_event_msg",
           flow_retry_times = 4, // 经过消息流重试次数
           func_retry_times = 1  //  当前执行方法内部直接串联循环重试次数
       };
       var flowProcessor = new FlowEventProcessor<TestCount,TestCount>(new TestEvent(), o);
       
       var countPara    = new TestCount() {count = 0};

       var countRes = await flowProcessor.Process(countPara);
       Assert.IsNull(countRes); // 首次抛出异常，拦截返回空

       await Task.Delay(5000);  // 异步消息队列消费缓冲

       // 总执行次数 = (flow_retry_times+1)*(func_retry_times+1) = (4+1)*(1+1) = 10
       Assert.IsTrue(countPara.count==10);// 默认消息队列实现是内存级，countPara引用不变
   	}
```

