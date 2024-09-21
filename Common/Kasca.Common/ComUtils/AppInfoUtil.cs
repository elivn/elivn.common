namespace Kasca.Common.ComUtils
{
    /// <summary>
    ///  当前应用基础信息
    /// </summary>
    public class AppInfoUtil
    {
        /// <summary>
        ///  app应用
        /// </summary>
        public static string AppCode { get; } = ConfigUtil.GetSection("AppInfo:AppCode")?.Value;

        /// <summary>
        ///  当前应用版本
        /// </summary>
        public static string AppVersion { get; } = ConfigUtil.GetSection("AppInfo:AppVersion")?.Value;

        /// <summary>
        /// App秘钥
        /// </summary>
        public static string AppSecret { get; } = ConfigUtil.GetSection("AppInfo:AppSecret")?.Value;

        /// <summary>
        /// 当前应用环境
        /// </summary>
        public static string AppEnvironment { get; } = ConfigUtil.GetSection("AppInfo:Environment")?.Value;
        
        /// <summary>
        /// 是否打开Http请求日志
        /// </summary>
        public static bool OpenHttpLog { get; } = ConfigUtil.GetSection("AppInfo:OpenHttpLog")?.Value=="Y";
    }
}
