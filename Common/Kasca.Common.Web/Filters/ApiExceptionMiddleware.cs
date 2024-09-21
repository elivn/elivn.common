using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;
using Kasca.Common.Plugs.LogPlug;
using Kasca.Common.Exceptions;

namespace Kasca.Common.Web.Filters
{
    /// <summary>
    ///  异常处理中间件
    /// </summary>
    public class ApiExceptionMiddleware : BaseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var sysInfo = new AppAuthorizeInfo();
       
            InitialAuthorizeInfoFromNew(context, sysInfo);
            MemberShiper.SetAppAuthrizeInfo(sysInfo);   //   最上层中间件中赋值，否则下边为空
            
            Exception error = null;
            ResultMo errRes = null;

            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                await _next.Invoke(context);
                if (context.Response.StatusCode != (int) HttpStatusCode.NotFound)
                    return;

                await ResponseEnd(context, new ResultMo(-1, "当前接口不存在！"));
            }
            catch (ApiException ex)
            {
                errRes = new ResultMo(ex.Code, ex.Msg);
                error = ex;
            }
            catch (Exception ex)
            {
                errRes = new ResultMo(ResultTypes.InnerError, string.Concat("出现未知错误，请求错误码：", MemberShiper.AppAuthorize?.LogSerialNum));
                error = ex;
            }
            finally
            {
                if (stopwatch.IsRunning)
                    stopwatch.Stop();

                var hasError = errRes != null;

                if (hasError)
                    await WriteGlobalError(context, error, errRes);

                HttpLogUtil.WriteHttpGlobalLogs(MemberShiper.AppAuthorize, context.Request.Method, stopwatch.ElapsedMilliseconds, hasError);
            }

        }

        private async Task WriteGlobalError(HttpContext context, Exception error, ResultMo errRes)
        {
            var logSerNum = MemberShiper.AppAuthorize?.LogSerialNum;

            var strMsg = new StringBuilder();
            strMsg.Append("全局请求编号：").Append(logSerNum).Append("\r\n");
            strMsg.Append("详细错误信息：").Append(error).Append("\r\n");

            LogUtil.Error(strMsg.ToString(), nameof(ApiExceptionMiddleware));
            await ResponseEnd(context, errRes);
        }
    }

    /// <summary>
    /// 异常处理中间件扩展类
    /// </summary>
    public static class ExceptionMiddlewareExtention
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiExceptionMiddleware>();
        }
    }
}