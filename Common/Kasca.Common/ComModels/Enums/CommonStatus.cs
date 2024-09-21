#region Copyright (C) 2017 Elivn  

/***************************************************************************
*　　	文件功能描述：OSSCore —— 通用状态枚举
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*    	创建日期：2017-5-16
*       
*****************************************************************************/

#endregion

using Kasca.Common.Extention;

namespace Kasca.Common.ComModels.Enums
{
    /// <summary>
    ///     通用状态码 （如果需要更多状态需要自定义枚举，此值不再新增）
    ///     不同的领域对象可能会一到多个
    /// </summary>
    public enum CommonStatus
    {
        /// <summary>
        ///     删除状态
        /// </summary>
        Delete = -1000,

        /// <summary>
        ///  自定义扩展  【取消】
        /// </summary>
        Canceled = -100,

        /// <summary>
        /// 自定义扩展   【审核失败】
        /// </summary>
        Failed = -10,

        /// <summary>
        ///   正常原始状态
        /// </summary>
        Original = 0,

        /// <summary>
        /// 初步提交待审核状态 【提交待确认】【提交待审核】
        /// </summary>
        WaitConfirm = 10,

        /// <summary>
        ///  确认通过激活状态  【确认成功】【审核通过】【已上架/线】
        /// </summary>
        Confirmed = 20,

        /// <summary>
        ///  完成结束状态
        /// </summary>
        Finished = 100
    }

    /// <summary>
    ///  交易类型
    /// </summary>
    public enum PaymentTransType
    {
        /// <summary>
        ///  订单
        /// </summary>
        Order,

        /// <summary>
        /// 退款申请
        /// </summary>
        RefundApply,

    }

    /// <summary>
    ///     贷款订单状态
    /// </summary>
    public enum LoanSheetStatus
    {
        #region 兼容旧申请审核阶段状态

        /// <summary>
        ///  【兼】 初始申请 
        /// </summary>
        [OSDescript("初始申请")] Apply_Start = 100,

        /// <summary>
        ///  【兼】 申请拒绝 
        /// </summary>
        [OSDescript("申请拒绝")] Apply_Start_Refusey = -100,

        /// <summary>
        ///  【兼】 申请【后】【弃单】 
        /// </summary>
        [OSDescript("申请【后】【弃单】")] Apply_Start_Abandon = -101,



        /// <summary>
        ///  【兼】 后台初审  
        /// </summary>
        [OSDescript("后台初审")] Audit_FirstTrial = 200,

        /// <summary>
        ///  【兼】 初审拒绝 
        /// </summary>
        [OSDescript("初审拒绝")] Audit_FirstTrial_Refuse = -200,

        /// <summary>
        ///  【兼】 初审【后】【弃单】 
        /// </summary>
        [OSDescript("初审【后】【弃单】")] Audit_FirstTrial_Abandon = -201,


        /// <summary>
        ///  【兼】 客户分配完成 
        /// </summary>
        [OSDescript("客户分配完成")] Audit_AppointProduct = 300,

        /// <summary>
        ///  【兼】 客户分配拒绝 
        /// </summary>
        [OSDescript("客户分配拒绝")] Audit_AppointProduct_Refuse = -300,

        /// <summary>
        ///  【兼】 客户分配【后】【弃单】 
        /// </summary>
        [OSDescript("客户分配【后】【弃单】")] Audit_AppointProduct_Abandon = -301,

        /// <summary>
        ///  【兼】 补充申请完成
        /// </summary>
        [OSDescript("补充申请完成")] Audit_CollectInfo = 400,

        /// <summary>
        ///  【兼】 补充申请拒绝 
        /// </summary>
        [OSDescript("补充申请拒绝")] Audit_CollectInfo_Refuse = -400,

        /// <summary>
        ///  【兼】 补充申请【后】【弃单】 
        /// </summary>
        [OSDescript("补充申请【后】【弃单】")] Audit_CollectInfo_Abandon = -401,

        /// <summary>
        ///  【兼】 客户分析完成 
        /// </summary>
        [OSDescript("客户分析完成")] Audit_Analysis = 500,

        /// <summary>
        ///  【兼】 客户分析拒单 
        /// </summary>
        [OSDescript("客户分析拒单")] Audit_Analysis_Refuse = -500,

        /// <summary>
        ///  【兼】 客户分析【弃单】 
        /// </summary>
        [OSDescript("客户分析【后】【弃单】")] Audit_Analysis_Abandon = -501,

        /// <summary>
        ///  【兼】 决议完成  
        /// </summary>
        [OSDescript("决议中拒绝")] Audit_Decisioning_Refuse = -1000,

        /// <summary>
        ///  【兼】 决议完成  
        /// </summary>
        [OSDescript("决议中弃单")] Audit_Decisioning_Abandon = -1001,


        /*
        中间值预留
         */
        /// <summary>
        ///  【兼】 决议完成  
        /// </summary>
        [OSDescript("决议完成")] Audit_Decision = 1100,

        /// <summary>
        ///  【兼】 贷款决议【后】客户【过期】 
        /// </summary>
        [OSDescript("贷款决议【后】【过期】")] Audit_Decision_Expired = -1102,

        /// <summary>
        ///  【兼】 贷款决议拒绝 
        /// </summary>
        [OSDescript("决议拒绝")] Audit_Decision_Refuse = -1100,

        /// <summary>
        ///  【兼】 贷款决议【后】客户【弃单】 
        /// </summary>
        [OSDescript("贷款决议【后】【弃单】")] Audit_Decision_Abandon = -1101,


        #endregion

        /// <summary>
        ///   贷款单待校验【初始状态】 
        /// </summary>
        [OSDescript("贷款单待校验")] Loan_AddSheet = 1104,

        /// <summary>
        ///   贷款单校验失败
        /// </summary>
        [OSDescript("贷款单校验失败")] Loan_ChannelCheckFailed = 1105,


        /// <summary>
        ///   贷款单校验成功-待确认
        /// </summary>
        [OSDescript("校验成功待确认")] Loan_ChannelCheckSuccess = 1110,

        /// <summary>
        ///   校验成功【后】【拒单】 
        /// </summary>
        [OSDescript("校验成功【后】【拒单】")] Loan_ChannelCheckSuccess_Refuse = -1110,

        /// <summary>
        ///  校验成功【后】【弃单】
        /// </summary>
        [OSDescript("校验成功【后】【弃单】")] Loan_ChannelCheckSuccess_Abandon = -1111,

        /// <summary>
        ///     贷款确认待绑卡 
        /// </summary>
        [OSDescript("贷款确认待绑卡")] Loan_UserConfirmed = 1200,

        /// <summary>
        ///  确认贷款【后】【拒单】
        /// </summary>
        [OSDescript("确认贷款【后】【拒单】")] Loan_UserConfirmed_Refuse = -1200,

        /// <summary>
        ///  确认贷款【后】【弃单】
        /// </summary>
        [OSDescript("确认贷款【后】【弃单】")] Loan_UserConfirmed_Abandon = -1201,

        /// <summary>
        ///	    绑卡中 (部分绑卡-通常已经绑定放款卡) 
        /// </summary>
        [OSDescript("绑卡中")] Loan_BankAccountBinding = 1300,

        /// <summary>
        ///  绑卡中【拒单】
        /// </summary>
        [OSDescript("绑卡中【拒单】")] Loan_BankAccountBinding_Refuse = -1300,

        /// <summary>
        ///  绑卡中【弃单】
        /// </summary>
        [OSDescript("绑卡中【弃单】")] Loan_BankAccountBinding_Abandon = -1301,

        /// <summary>
        ///   绑卡完成  
        /// </summary>
        [OSDescript("绑卡完成")] Loan_BankAccountBound = 1400,

        /// <summary>
        ///   绑卡完成【后】【拒单】
        /// </summary>
        [OSDescript("绑卡完成【后】【拒单】")] Loan_BankAccountBound_Refuse = -1400,

        /// <summary>
        ///  绑卡完成【后】【弃单】
        /// </summary>
        [OSDescript("绑卡完成【后】【弃单】")] Loan_BankAccountBound_Abandon = -1401,

        /// <summary>
        ///   绑卡确认(待签署合同)
        /// </summary>
        [OSDescript("绑卡确认（待签署合同）")] Loan_BankAccountConfirmed = 1410,

        /// <summary>
        ///   绑卡确认【后】【拒单】
        /// </summary>
        [OSDescript("绑卡确认【后】【拒单】")] Loan_BankAccountConfirmed_Refuse = -1410,

        /// <summary>
        ///  绑卡确认【后】【弃单】
        /// </summary>
        [OSDescript("绑卡确认【后】【弃单】")] Loan_BankAccountConfirmed_Abandon = -1411,

        /// <summary>
        ///     已生成合同(未签署合同) 
        /// </summary>
        [OSDescript("已生成合同")] Loan_ContractGenerate = 1500,

        /// <summary>
        /// 生成合同【后】【拒单】
        /// </summary>
        [OSDescript("生成合同【后】【拒单】")] Loan_ContractGenerate_Refuse = -1500,

        /// <summary>
        /// 生成合同【后】【弃单】
        /// </summary>
        [OSDescript("生成合同【后】【弃单】")] Loan_ContractGenerate_Abandon = -1501,
        
        /// <summary>
        ///     共借人承诺函待签署(主贷合同已签署)
        /// </summary>
        [OSDescript("共借人承诺函待签署")] Loan_CommitmentLetterGenerate = 1510,

        /// <summary>
        ///     共借人承诺函待签署(主贷合同已签署)
        /// </summary>
        [OSDescript("共借人承诺函待签署阶段【拒单】")] Loan_CommitmentLetterGenerate_Refuse = -1510,

        /// <summary>
        ///     共借人承诺函待签署(主贷合同已签署)
        /// </summary>
        [OSDescript("共借人承诺函待签署阶段【弃单】")] Loan_CommitmentLetterGenerateAbandon = -1511,


        /// <summary>
        ///	    已签署合同(未确认放款) 
        /// </summary>
        [OSDescript("已签署合同")] Loan_ContractSign = 1600,

        /// <summary>
        /// 签署合同【后】【拒单】
        /// </summary>
        [OSDescript("签署合同【后】【拒单】")] Loan_ContractSign_Refuse = -1600,

        /// <summary>
        /// 签署合同【后】弃单
        /// </summary>
        [OSDescript("签署合同【后】【弃单】")] Loan_ContractSign_Abandon = -1601,

        /// <summary>
        ///     确认放款
        /// </summary>
        [OSDescript("确认放款")] Loan_DeliverMoneyConfirmed = 1700,

        /// <summary>
        /// 确认放款【后】【拒单】
        /// </summary>
        [OSDescript("确认放款【后】【拒单】")] Loan_DeliverMoneyConfirmed_Refuse = -1700,

        /// <summary>
        /// 确认放款【后】【弃单】
        /// </summary>
        [OSDescript("确认放款【后】【弃单】")] Loan_DeliverMoneyConfirmed_Abandon = -1701,

        /// <summary>
        ///     渠道放款失败
        /// </summary>
        [OSDescript("渠道放款【失败】")] Loan_DeliverMoney_Fail = 1795,

        /// <summary>
        ///     放款成功还款中
        /// </summary>
        [OSDescript("放款成功还款中")] Loan_DeliverMoney_Success = 1800,

        /// <summary>
        ///	    还款已结清
        /// </summary>
        [OSDescript("还款已结清")] Loan_SheetClosed = 1900,

        /// <summary>
        /// 【慎用】 系统层面拒绝 - 非特殊情况不要使用
        /// </summary>
        [OSDescript("系统层面拒绝")] Loan_Unkonw_Refuse = -1
    }
    
    
  
}