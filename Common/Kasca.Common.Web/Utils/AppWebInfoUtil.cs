using Kasca.Common.ComUtils;

namespace Kasca.Common.Web.Utils
{
    /// <summary>
    ///  web 应用的基础配置信息
    /// </summary>
    public class AppWebInfoUtil : AppInfoUtil
    {
        /// <summary>
        ///  登录页
        /// </summary>
        public static string LoginUrl { get; } = ConfigUtil.GetSection("AppWebInfo:LoginUrl")?.Value;

        /// <summary>
        ///  404页面
        /// </summary>
        public static string NotFoundUrl { get; } = ConfigUtil.GetSection("AppWebInfo:NotFoundUrl")?.Value;


    }

}
