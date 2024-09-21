using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;
using Kasca.Common.ComUtils;
using Kasca.Common.Plugs.JWTPlug;

namespace Kasca.Common.Web.Filters
{
    /// <summary>
    ///  应用初始化处理
    /// </summary>
    public class ApiAppStartMiddlerware : BaseMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="next"></param>
        public ApiAppStartMiddlerware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.StartsWith("/swagger/"))
                return;

            if ( context.Request.Path.Value.StartsWith("/callapi/"))
            {
                await _next.Invoke(context);
                return;
            }

            var sysInfo = MemberShiper.AppAuthorize;
            if (sysInfo == null)
            {
                sysInfo = new AppAuthorizeInfo();
                InitialAuthorizeInfoFromNew(context,sysInfo);
                MemberShiper.SetAppAuthrizeInfo(sysInfo);
            }
            
            if (string.IsNullOrEmpty(sysInfo.AppCode))
            {
                await ResponseEnd(context, new ResultMo(ResultTypes.UnKnowSource, "未知应用来源！"));
                return;
            }
            
            // 3. 检验签名信息
            var checkRes = CheckAppSign(sysInfo);
            if (!checkRes.IsSuccess())
            {
                await ResponseEnd(context, checkRes);
                return;
            }

            // 4. 补充应用信息部分
            CompleteAuthorizeInfo(sysInfo ,context);

            // 5. 初始化应用登录用户信息
            InitailMemberIdentity(sysInfo);

            //此处可以添加身份验证

            //  》》 执行生命周期核心部分
            await _next.Invoke(context);
        }

        private static void CompleteAuthorizeInfo(AppAuthorizeInfo sysInfo, HttpContext context)
        {
            if (string.IsNullOrEmpty(sysInfo.LogSerialNum))
            {
                sysInfo.LogSerialNum = context.TraceIdentifier;
            }
            
            if (string.IsNullOrEmpty(sysInfo.IpAddress))
            {
                string ipAddress = context.Request.Headers["X-Forwarded-For"];
                sysInfo.IpAddress = !string.IsNullOrEmpty(ipAddress)
                    ? ipAddress
                    : context.Connection.RemoteIpAddress.ToString();
            }
        }

        private static ResultMo CheckAppSign(AppAuthorizeInfo sysInfo)
        {
            if (AppInfoUtil.AppEnvironment != "Product")
                return new ResultMo();

            var keyRes = AppSecretUtil.GetAppSecret(sysInfo.AppCode);
            if (!keyRes.IsSuccess())
                return new ResultMo(ResultTypes.UnKnowSource, "非法应用来源！");

            return sysInfo.CheckSign(keyRes.data)
                ? new ResultMo()
                : new ResultMo(ResultTypes.SignError, "应用签名错误！");
        }

        #region 初始化系统信息部分

        /// <summary>
        ///  初始化用户信息
        /// </summary>
        /// <param name="sysInfo"></param>
        private static void InitailMemberIdentity(AppAuthorizeInfo sysInfo)
        {
            //  todo  待抽取到专门的用户信息处理方法
            var userInfo = new MemberIdentity
            {
                UserId = sysInfo.UserId,
                EmployeeId = sysInfo.EmpCode,
                MemberInfo= JWTPlug.ValidJwtToken()
            };
            MemberShiper.SetIdentity(userInfo);
        }

        

        #endregion
    }

    /// <summary>
    ///  App秘钥辅助类 
    /// </summary>
    public static class AppSecretUtil
    {
        private static readonly ConcurrentDictionary<string,string> _appSecrets=new ConcurrentDictionary<string, string>();
        
        /// <summary>
        ///   获取对应的
        /// </summary>
        /// <param name="appCode"></param>
        /// <returns></returns>
        public static ResultMo<string> GetAppSecret(string appCode)
        {
            _appSecrets.TryGetValue(appCode,out var secret);

            if (string.IsNullOrEmpty(secret))
            {
                //  todo 待修改为从应用中心获取
                //  todo 待优化，添加一个小时黑名单过滤，然后再远程获取
                secret = ConfigUtil.GetSection($"AppSecrets:{appCode}")?.Value;
            }

            if (string.IsNullOrEmpty(secret))
            {
                return new ResultMo<string>(ResultTypes.UnKnowSource, "未知应用来源！");
            }

            _appSecrets.TryAdd(appCode, secret);
            return new ResultMo<string>(secret);
        }
    }

    /// <summary>
    /// 全局信息初始化中间件扩展类
    /// </summary>
    public static class AppStartMiddlewareExtention
    {
        /// <summary>
        ///  
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAppStartMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiAppStartMiddlerware>();
        }
    }
}
