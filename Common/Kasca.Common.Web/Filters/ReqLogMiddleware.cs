using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Kasca.Common.Web.Filters
{
    /// <summary>
    ///  应用初始化处理
    /// </summary>
    public class ReqLogMiddlerware : BaseMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="next"></param>
        public ReqLogMiddlerware(RequestDelegate next) 
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
            var logNum = context.TraceIdentifier;

            //  写请求记录日志
            await HttpLogUtil.WriteHttpReqLog(context, logNum);

            //  》》 执行生命周期核心部分
            await _next.Invoke(context);
        }
    }

   
    /// <summary>
    /// 全局信息初始化中间件扩展类
    /// </summary>
    public static class ReqLogMiddlewareExtention
    {
        /// <summary>
        ///  请求日志扩展
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReqLogMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ReqLogMiddlerware>();
        }
    }
}
