using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.ComModels;
using Kasca.Common.ComUtils;
using Kasca.OrmPlug.Oracle;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class OralceTests:BaseTests
    {
        [TestMethod]
        public async Task PostTest()
        {
            var res =await LoanSheetOracleRep.Instance.GetListByCustomerGuidProjectId();
           
            // 12 4095
            // 11 2047
            // 10 1023
            // 9  511
            // 53    9007199254740991

            //Console.WriteLine(-1L^(-1L<<53));
            //Console.ReadLine();

            //var jsNumG=new JsNum16LenGenerator(0);
            //var id = jsNumG.NextNum();

            //var sam1 = new SampleTest() { Id = NumUtil.SnowNum(), name = "SnowNum" };
            //var add1Res =await OracleRep.Instance.Add(sam1);
            //var get1Res = await OracleRep.Instance.GetById(sam1.Id.ToString());

            //var sam2 = new SampleTest() { Id = NumUtil.SnowNumJs16(), name = "SnowNum16" };
            //var add2Res = await OracleRep.Instance.Add(sam2);
            //var get2Res = await OracleRep.Instance.GetById(sam2.Id.ToString());

            //var sam3 = new SampleTest() { Id = (-1L ^ (-1L << 53)), name = "2 ^ 52" };
            //var add3Res = await OracleRep.Instance.Add(sam3);
            //var get3Res = await OracleRep.Instance.GetById(sam3.Id.ToString());
        }
    }

    public class LoanTestMo
    {
        //
        public string Guid { get; set; }

        public string EtimeId { get; set; }

        public string Status { get; set; }

        public int LoanStatus { get; set; }
    }

    public class LoanSheetOracleRep : BaseOracleRep<LoanSheetOracleRep, TimeAssetMo>
    {
        public LoanSheetOracleRep()
        {
            m_writeConnectionString = "User Id=ts170201;Password=Xts$739k;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=10.18.32.122)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=tstdb)))";
            m_readeConnectionString = "User Id=ts170201;Password=Xts$739k;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=10.18.32.122)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=tstdb)))";
            m_TableName = "T_TimeAsset";
        }
        
        public async Task<ResultListMo<TimeAssetMo>> GetListByCustomerGuidProjectId()
        {
            string sqlstr = @"select * from T_TimeAsset t where t.customerguid=:customerGuid AND t.feeProjectId=:feeProjectId ";
            var dicPara = new Dictionary<string, object>
            {
                { ":customerGuid", "{BFD752A7-690F-454C-99AC-12C56DAB2697}" },
                { ":feeProjectId", "120826047703810048"  },

            };

            var r = await GetList(sqlstr, dicPara);
            return r;
        }
    }

    /// <summary>
    /// 用户时效资产
    /// </summary>
    public class TimeAssetMo : BaseMo
    {
        /// <summary>
        /// 客户Id CustomerGuid
        /// </summary>
        [Required]
        public string CustomerGuid { get; set; }

        /// <summary>
        /// 资产名称(存的是费用项名称)
        /// </summary>
        [Required]
        public string FeeProjectName { get; set; }
        /// <summary>
        /// 费用项Id
        /// </summary>
        [Required]
        public string FeeProjectId { get; set; }
        /// <summary>
        /// 费用项版本id
        /// </summary>
        [Required]
        public string FeeProjectVerId { get; set; }
        /// <summary>
        /// （费用项）区间Id
        /// </summary>
        public string FeeStageId { get; set; }


        /// <summary>
        /// 月单价
        /// </summary>
        public decimal PricePerMonth { get; set; }
        /// <summary>
        /// 实收月单价
        /// </summary>
        public decimal RealPricePerMonth { get; set; }
        /// <summary>
        /// 原价应收费用金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 实收费用
        /// </summary>
        public decimal RealPayMoney { get; set; }
        /// <summary>
        /// 购买期数（如果是分段的，则只是本段的期数）
        /// </summary>
        public decimal BuyTimes { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 状态 值域:enum AssetStatus ；
        /// </summary>
        [Required]
        public int Status { get; set; }


        /// <summary>
        /// 资产编号/会员卡号/V+卡号
        /// </summary>
        public string AssetNum { get; set; }

        /// <summary>
        /// AppCode
        /// </summary>
        [Required]
        public string AppCode { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        [Required]
        public string EmployeeGuid { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        [Required]
        public string EmployeeName { get; set; }
        /// <summary>
        /// 提贷申请单Id
        /// </summary>        
        [Required]
        public string SheetApplyId { get; set; }
        /// <summary>
        /// 授信Id
        /// </summary>
        public string CreditId { get; set; }
        /// <summary>
        /// 购买时用的费用包Id
        /// </summary>
        public string FeePackId { get; set; }
        /// <summary>
        /// 被覆盖的时效资产段Id
        /// </summary>
        public string BCoveredId { get; set; }
        /// <summary>
        /// 银行贷guid（同步旧的数据用）
        /// </summary>
        public string LoanGuid { get; set; }
        /// <summary>
        /// 安分期贷款Guid（同步旧的数据用）
        /// </summary>
        public string PayLoanGuid { get; set; }
        /// <summary>
        /// 支付费用的安分期贷款单Id 没有大括号{}
        /// </summary>
        public string PayLoanSheetId { get; set; }
        /// <summary>
        /// 放款银行guid（如果是放款渠道专用费用包则有值）
        /// </summary>
        public string LoanBankGuid { get; set; }
    }

    public class SampleTest
    {
        //{a8b6d464-e445-4225-a32e-2dc7b85bfe2f}
        public long Id { get; set; }

        public string name { get; set; }
        
    }
    public class OracleRep:BaseOracleRep<OracleRep, SampleTest>
    {
        public OracleRep()
        {
            m_writeConnectionString = "User Id=ts170201;Password=Xts$739k;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=10.18.32.122)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=tstdb)))";
            m_readeConnectionString = "User Id=ts170201;Password=Xts$739k;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=10.18.32.122)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=tstdb)))";
            m_TableName = "TEST_SMAPLETEST";
        }


    }


}
