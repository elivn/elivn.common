namespace Kasca.Common.ComModels.Enums
{
    /// <summary>
    /// 交易金额类型
    /// </summary>
    public enum TransType
    {
        /// <summary>
        /// 实收
        /// </summary>
        收款_实收 = 0,

        /// <summary>
        /// 减免
        /// </summary>
        收款_减免 = 10,

        /// <summary>
        /// 
        /// </summary>
        内部结转=100,

        /// <summary>
        /// 退款
        /// </summary>
        退款 = 200
    }
}
