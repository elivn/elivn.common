using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;
using Kasca.OrmPlug.SqlServer;
using Kasca.CachePlug.Redis;
using Kasca.Common.Plugs.CachePlug;
using System.Text;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class SqlTests : BaseTests
    {


        [TestMethod]
        public async Task TestSearchFeature()
        {
            StringBuilder builder = new StringBuilder("host: ").Append("api.xf-yun.com").Append("\n").//
                Append("date: ").Append("Fri, 23 Apr 2021 02:35:47 GMT").Append("\n").//
                Append("POST ").Append("/v1/private/s782b4996").Append(" HTTP/1.1");

            string signature = Kasca.Common.Encrypt.HMACSHA.EncryptBase64(builder.ToString(), "apisecretXXXXXXXXXXXXXXXXXXXXXXX", null, "SHA256");

            var testdata = signature;
        }



        [TestMethod]
        public async Task PostTest2()
        {
            SchoolEnvironmentRecordMo stuPhysicalConditionRecord = new SchoolEnvironmentRecordMo();
            stuPhysicalConditionRecord.CreateTime = DateTime.Now;
            stuPhysicalConditionRecord.IsDel = false;
            stuPhysicalConditionRecord.TemperatureWarn = 0;
            stuPhysicalConditionRecord.HumidityWarn = 0;
            stuPhysicalConditionRecord.PM25Warn = 0;
            stuPhysicalConditionRecord.TVOCWarn = 0;
            stuPhysicalConditionRecord.CarbonDioxideWarn = 0;
            stuPhysicalConditionRecord.FormaldehydeWarn = 0;
            var result = await SchoolEnvironmentRecordRep.Instance.Add(stuPhysicalConditionRecord);
        }

        [TestMethod]
        public async Task PostTest2Batch()
        {
            AbilityDescribeMo stuPhysicalConditionRecord = new AbilityDescribeMo();
            stuPhysicalConditionRecord.Id = 0;
            stuPhysicalConditionRecord.CreateTime = DateTime.Now;
            stuPhysicalConditionRecord.IsDel = false;
            stuPhysicalConditionRecord.AbilityDescribelId = "12324d";
            stuPhysicalConditionRecord.Title = "11";
            stuPhysicalConditionRecord.ParentAbilityClassifyId = "AbilityClassify-00163E02237D-Kasca1_Server_1-20200612170924892-1410016";
            stuPhysicalConditionRecord.ClassId = "Class-Kasca.ksj.Service(1)-0000000000w-Class-00008";
            stuPhysicalConditionRecord.Content2 = "Describe";
            stuPhysicalConditionRecord.SchoolId = "School-00E04C2C4E57-Kasca.ksj.Service(1)-20200903105505700-4234699";
            stuPhysicalConditionRecord.CreateBy = "SysUser-00E04C2C4E57-Kasca.ksj.Service(1)-20200901190412905-1112687";
            stuPhysicalConditionRecord.Describe = "Describe";
            stuPhysicalConditionRecord.UpdateBy = "123";
            stuPhysicalConditionRecord.UpdateTime = DateTime.Now;
            List<AbilityDescribeMo> lst = new List<AbilityDescribeMo>();
            lst.Add(stuPhysicalConditionRecord);

            var result = await AbilityDescribeRep.Instance.AddBatch(lst);
            var i = 1;
        }

        [TestMethod]
        public async Task GetRedis()
        {
            EventThreshodMo tModel = new EventThreshodMo();
            tModel.Id = 1234;
            Dictionary<string, EventThreshodMo> dct = new Dictionary<string, EventThreshodMo>();
            dct.Add("123", tModel);
            dct.Add("234", tModel);
            CachePlug.Redis.StackRedisPlug.Instance.Set("Bearer 123@@", tModel, TimeSpan.FromMinutes(5));
            CachePlug.Redis.StackRedisPlug.Instance.Set("test2", dct, TimeSpan.FromMinutes(5));
            var result6 = CachePlug.Redis.StackRedisPlug.Instance.Get<Dictionary<string, EventThreshodMo>>("test2");
            var result= CachePlug.Redis.StackRedisPlug.Instance.Get<EventThreshodMo>("test");
            var result2 = CachePlug.Redis.StackRedisPlug.Instance.Remove("Bearer 123@@");
            var result3 = CachePlug.Redis.StackRedisPlug.Instance.Remove("test3");

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

    public class AbilityDescribeMo
    {
        /// <summary>
        ///编号
        /// </summary>
        public long Id
        {
            get;
            set;
        }
        /// <summary>
        ///外编号
        /// </summary>
        public string AbilityDescribelId
        {
            get;
            set;
        }
        /// <summary>
        ///标题 标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        ///能力领域Id
        /// </summary>
        public string ParentAbilityClassifyId
        {
            get;
            set;
        }
        /// <summary>
        ///班级Id
        /// </summary>
        public string ClassId
        {
            get;
            set;
        }
        /// <summary>
        ///学校Id
        /// </summary>
        public string SchoolId
        {
            get;
            set;
        }
        /// <summary>
        ///描述
        /// </summary>
        public string Describe
        {
            get;
            set;
        }
        /// <summary>
        ///内容详情
        /// </summary>
        public string Content2
        {
            get;
            set;
        }
        /// <summary>
        ///创建人编号 当前用户ID
        /// </summary>
        public string CreateBy
        {
            get;
            set;
        }
        /// <summary>
        ///创建日期 默认为当前时间
        /// </summary>
        public Nullable<System.DateTime> CreateTime
        {
            get;
            set;
        }
        /// <summary>
        ///修改人编号 当前用户ID
        /// </summary>
        public string UpdateBy
        {
            get;
            set;
        }
        /// <summary>
        ///修改日期 默认为当前时间
        /// </summary>
        public Nullable<System.DateTime> UpdateTime
        {
            get;
            set;
        }
        /// <summary>
        ///是否已删除 0正常 1已删除
        /// </summary>
        public bool IsDel
        {
            get;
            set;
        }
    }

    public class SchoolEnvironmentRecordMo
    {
        public long Id { get; set; }
        public string SchoolEnvironmentRecordId { get; set; }
        public string SchoolId { get; set; }
        public Nullable<double> Temperature { get; set; }
        public Nullable<int> TemperatureWarn { get; set; }
        public Nullable<double> Humidity { get; set; }
        public Nullable<int> HumidityWarn { get; set; }
        public Nullable<double> PM25 { get; set; }
        public Nullable<int> PM25Warn { get; set; }
        public Nullable<double> TVOC { get; set; }
        public Nullable<int> TVOCWarn { get; set; }
        public Nullable<double> CarbonDioxide { get; set; }
        public Nullable<int> CarbonDioxideWarn { get; set; }
        public Nullable<double> Formaldehyde { get; set; }
        public Nullable<int> FormaldehydeWarn { get; set; }
        public string LocationType { get; set; }
        public string ClassId { get; set; }
        public Nullable<bool> IsDel { get; set; }
        public string FromSource { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
    }
    public class SchoolEnvironmentRecordRep : BaseSqlServerRep<SchoolEnvironmentRecordRep, SchoolEnvironmentRecordMo>
    {
        public SchoolEnvironmentRecordRep()
        {
            m_writeConnectionString = "Server=120.24.30.132;Database=Kasca;Uid=kasca;Pwd=123@Kasca.com";
            m_readeConnectionString = "Server=120.24.30.132;Database=Kasca;Uid=kasca;Pwd=123@Kasca.com";
            m_TableName = "t_SchoolEnvironmentRecord";
        }
       
    }

    public class AbilityDescribeRep : BaseSqlServerRep<AbilityDescribeRep, AbilityDescribeMo>
    {
        public AbilityDescribeRep()
        {
            m_writeConnectionString = "Server=120.24.30.132;Database=Kasca;Uid=kasca;Pwd=123@Kasca.com";
            m_readeConnectionString = "Server=120.24.30.132;Database=Kasca;Uid=kasca;Pwd=123@Kasca.com";
            m_TableName = "t_AbilityDescribe";
        }

    }
}
