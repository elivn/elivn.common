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
    /// �û�ʱЧ�ʲ�
    /// </summary>
    public class TimeAssetMo : BaseMo
    {
        /// <summary>
        /// �ͻ�Id CustomerGuid
        /// </summary>
        [Required]
        public string CustomerGuid { get; set; }

        /// <summary>
        /// �ʲ�����(����Ƿ���������)
        /// </summary>
        [Required]
        public string FeeProjectName { get; set; }
        /// <summary>
        /// ������Id
        /// </summary>
        [Required]
        public string FeeProjectId { get; set; }
        /// <summary>
        /// ������汾id
        /// </summary>
        [Required]
        public string FeeProjectVerId { get; set; }
        /// <summary>
        /// �����������Id
        /// </summary>
        public string FeeStageId { get; set; }


        /// <summary>
        /// �µ���
        /// </summary>
        public decimal PricePerMonth { get; set; }
        /// <summary>
        /// ʵ���µ���
        /// </summary>
        public decimal RealPricePerMonth { get; set; }
        /// <summary>
        /// ԭ��Ӧ�շ��ý��
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// ʵ�շ���
        /// </summary>
        public decimal RealPayMoney { get; set; }
        /// <summary>
        /// ��������������Ƿֶεģ���ֻ�Ǳ��ε�������
        /// </summary>
        public decimal BuyTimes { get; set; }
        /// <summary>
        /// ��Ч��ʼʱ��
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// ��Ч����ʱ��
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// ״̬ ֵ��:enum AssetStatus ��
        /// </summary>
        [Required]
        public int Status { get; set; }


        /// <summary>
        /// �ʲ����/��Ա����/V+����
        /// </summary>
        public string AssetNum { get; set; }

        /// <summary>
        /// AppCode
        /// </summary>
        [Required]
        public string AppCode { get; set; }
        /// <summary>
        /// Ա��Id
        /// </summary>
        [Required]
        public string EmployeeGuid { get; set; }
        /// <summary>
        /// Ա������
        /// </summary>
        [Required]
        public string EmployeeName { get; set; }
        /// <summary>
        /// ������뵥Id
        /// </summary>        
        [Required]
        public string SheetApplyId { get; set; }
        /// <summary>
        /// ����Id
        /// </summary>
        public string CreditId { get; set; }
        /// <summary>
        /// ����ʱ�õķ��ð�Id
        /// </summary>
        public string FeePackId { get; set; }
        /// <summary>
        /// �����ǵ�ʱЧ�ʲ���Id
        /// </summary>
        public string BCoveredId { get; set; }
        /// <summary>
        /// ���д�guid��ͬ���ɵ������ã�
        /// </summary>
        public string LoanGuid { get; set; }
        /// <summary>
        /// �����ڴ���Guid��ͬ���ɵ������ã�
        /// </summary>
        public string PayLoanGuid { get; set; }
        /// <summary>
        /// ֧�����õİ����ڴ��Id û�д�����{}
        /// </summary>
        public string PayLoanSheetId { get; set; }
        /// <summary>
        /// �ſ�����guid������Ƿſ�����ר�÷��ð�����ֵ��
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
