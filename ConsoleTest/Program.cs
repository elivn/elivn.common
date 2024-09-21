using Kasca.Common.Authrization;
using Kasca.Common.ComUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            BaseTests();
            Console.WriteLine("Hello World!");
            GetRedis().Wait();
        }
        static void BaseTests()
        {
            SetConfig();
            MemberShiper.SetAppAuthrizeInfo(new AppAuthorizeInfo());
        }

        private static void SetConfig()
        {
            var basePat = Directory.GetCurrentDirectory();
         //   var configPath = basePat.Substring(0, basePat.IndexOf("bin"));
            var config = new ConfigurationBuilder()
                .SetBasePath(basePat)
                .Add(new JsonConfigurationSource
                {
                    Path = "appsettings.json",
                    ReloadOnChange = true
                }).Build();

            ConfigUtil.Configuration = config;

        }

        public static async Task GetRedis()
        {
            EventThreshodMo tModel = new EventThreshodMo();
            tModel.Id = 1234;
            Dictionary<string, EventThreshodMo> dct = new Dictionary<string, EventThreshodMo>();
            dct.Add("123", tModel);
            dct.Add("234", tModel);
            Kasca.Common.Plugs.CachePlug.StackRedisPlug.Instance.Set("Bearer 123@@", tModel, TimeSpan.FromMinutes(5));
            Kasca.Common.Plugs.CachePlug.StackRedisPlug.Instance.Set("test2", dct, TimeSpan.FromMinutes(5));
            var result6 = Kasca.Common.Plugs.CachePlug.StackRedisPlug.Instance.Get<Dictionary<string, EventThreshodMo>>("test2");
            Console.WriteLine(result6);
           
            var result = Kasca.Common.Plugs.CachePlug.StackRedisPlug.Instance.Get<EventThreshodMo>("test2");
            Console.WriteLine(result);

            Console.ReadLine();

            var i = 1;
        }
    }


    public class EventThreshodMo
    {
        public long Id { get; set; }
        public string EventThreshodId { get; set; }
        public int EventCategory { get; set; }
        public string EventThreshodName { get; set; }
        public decimal MinNum { get; set; }
        public decimal MaxNum { get; set; }
        public Nullable<int> IsEnable { get; set; }
        public string ReMark { get; set; }
        public Nullable<bool> IsDel { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
    }

}
