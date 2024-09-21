using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Kasca.Common.Extention;

namespace Kasca.Common.ComModels.Enums
{

    /// <summary>
    /// 协议类型
    /// </summary>
    public enum AgreementCategory
    {
        /// <summary>
        /// 在线支付
        /// </summary>
        [Description("在线支付")]
        OnlinePayment = 1,
        /// <summary>
        /// 快捷支付
        /// </summary>
        [Description("快捷支付")]
        QuickPayment = 2,
        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")]
        WechatPayment = 3,
        /// <summary>
        /// 委托代扣协议-中安信业
        /// </summary>
        [Description("委托代扣协议-中安信业")]
        RepaymentContract_ZAXY = 4,
        /// <summary>
        /// 委托代扣协议-中安小贷
        /// </summary>
        [Description("委托代扣协议-中安小贷")]
        RepaymentContract_ZAXD = 5,
        /// <summary>
        /// 委托代扣协议-中安安和网
        /// </summary>
        [Description("委托代扣协议-中安安和网")]
        RepaymentContract_ZAAH = 6,
        /// <summary>
        /// 中广核【授权注册协议】
        /// </summary>
        [Description("中广核【授权注册协议】")]
        RegisterAgreement_ZGH = 7,
        /// <summary>
        /// 贷款费用协议-v+卡
        /// </summary>
        [Description("贷款费用协议-v+卡")]
        LoanFeeContract_VPlus = 8,
        /// <summary>
        /// 贷款费用协议-保证金
        /// </summary>
        [Description("贷款费用协议-保证金")]
        LoanFeeContract_Bail = 9,
        /// <summary>
        /// 贷款费用协议-保证金
        /// </summary>
        [Description("委托贷款协议（通用）")]
        RepaymentContract = 10,
        /// <summary>
        /// 碧有信【授权注册协议】
        /// </summary>
        [Description("碧有信【授权注册协议】")]
        RegisterAgreement_BYX = 11,
        /// <summary>
        /// 佳兆业【授权注册协议】
        /// </summary>
        [Description("佳兆业【授权注册协议】")]
        RegisterAgreement_JZY = 12

    }

    /// <summary>
    /// 代扣协议类型
    /// </summary>
    public enum WithholdProcotol
    {
        /// <summary>
        /// 信业
        /// </summary>
        ZAXY,
        /// <summary>
        /// 小贷
        /// </summary>
        ZAXD,
        /// <summary>
        /// 安和
        /// </summary>
        ZAAH
    }
}
