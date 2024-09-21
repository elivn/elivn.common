using System;
using System.Collections.Generic;
using System.Text;

namespace Kasca.Common.Authrization
{
    /// <summary>
    /// JWT 实体声明
    /// </summary>
    public class SysUserJwtPayloadMo
    {
        /// <summary>
        /// 系统用户编号
        /// </summary>

        public string SysUserId { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>

        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>

        public string Nickname { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>

        public Nullable<int> SysRoleType { get; set; }

        /// <summary>
        /// 登录请求IP
        /// </summary>

        public string IP { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>

        public decimal exp { get; set; }

        /// <summary>
        /// 登录日志主键编号
        /// </summary>

        public string LoginLogoutTimeId { get; set; }

        /// <summary>
        /// 登录客户端
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 用户身份
        /// </summary>
        public string IdentityType { get; set; }

        /// <summary>
        /// 身份编号
        /// </summary>
        public string IdentityId { get; set; }
    }

    /// <summary>
    ///  JWT TOKEN 实体
    /// </summary>
    public class TokenMo
    {
        /// <summary>
        /// 生成的token
        /// </summary>

        public string TokenValue { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>

        public string SysUserId { get; set; }

        /// <summary>
        /// 登录客户端
        /// </summary>

        public Nullable<int> ClientName { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>

        public Nullable<DateTime> Expires { get; set; }
    }
}
