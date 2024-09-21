namespace Kasca.Common.ComModels.Enums
{
    /// <summary>
    ///   系统元素id类型
    /// </summary>
    public enum ElementIdType
    {
        /// <summary>
        ///     用户id Customerguid
        /// </summary>
        CustomerGuid = 0,

        /// <summary>
        ///     授信单Id EntryId
        /// </summary>
        EntryId = 10,

        /// <summary>
        ///     提贷单 SheetApplyId
        /// </summary>
        SheetApplyId = 20,

        /// <summary>
        ///     贷款单 loansheetId(原 EtimeId)
        /// </summary>
        LoanSheetId = 30,

        /// <summary>
        ///     贷款单loanguid
        /// </summary>
        LoanGuid = 40,

        /// <summary>
        ///  订单Id
        /// </summary>
        OrderId = 50,

        /// <summary>
        /// 费用项Id
        /// </summary>
        FeeProjectId = 60,

        ///<summary>
        ///  合同id ContractId
        /// </summary>
        ContractId = 70,

        /// <summary>
        ///  分期id InstalmentId
        /// </summary>
        InstalmentId = 80,

        /// <summary>
        ///  银叶通id YytOrderId
        /// </summary>
        YytOrderId = 90
    }

    public static class ElementIdTypeExtention
    {
        public static void InitialFromOldLoanOrLoanSheetId(ref this ElementIdType eType, string loanOrLoanSheetId)
        {
            if (loanOrLoanSheetId.StartsWith("Ins_"))
                eType = ElementIdType.InstalmentId;
            else if (loanOrLoanSheetId.Length == 38)
                eType = ElementIdType.LoanGuid;
            else if (loanOrLoanSheetId.Length == 36)
                eType = ElementIdType.LoanSheetId;
            else
                eType = ElementIdType.LoanSheetId;
        }
    }
}
