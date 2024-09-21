using System;

namespace Kasca.Common.ComModels
{
    /// <summary>
    ///  基础审计带状态实体
    /// </summary>
    public class BaseAuditStatusMo : BaseAuditMo
    {
        /// <summary>
        /// 业务状态
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// 基础审计实体
    /// </summary>
    public class BaseAuditMo : BaseMo<string>
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DeleteTime { get; set; }
        /// <summary>
        /// 删除人
        /// </summary>
        public string DeleteBy { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }
    }
}
