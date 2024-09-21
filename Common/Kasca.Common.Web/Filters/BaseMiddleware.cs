using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.Extention;

namespace Kasca.Common.Web.Filters
{
    /// <summary>
    ///  中间件基类
    /// </summary>
    public class BaseMiddleware
    {
        /// <summary>
        ///   结束Api请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        protected virtual async Task ResponseEnd(HttpContext context, ResultMo res)
        {
            ClearCacheHeaders(context.Response);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int) HttpStatusCode.OK;

            var respBody = $"{{\"code\":{res.code},\"msg\":\"{res.msg}\",\"interalTime\":\"{DateTime.Now.Ticks - (long)context.Items["request_time"]}\",\"EndTime\":\"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\",\"reqTime\":\"{context.Items["reqTime"]}\"}}";
            await context.Response.WriteAsync(respBody);

            HttpLogUtil.WriteHttpRespLog(context,respBody, MemberShiper.AppAuthorize?.LogSerialNum);
        }

        /// <summary>
        ///  清理Response缓存
        /// </summary>
        /// <param name="httpResponse"></param>
        protected static void ClearCacheHeaders(HttpResponse httpResponse)
        {
            httpResponse.Headers.Clear();
            httpResponse.Headers["Cache-Control"] = "no-cache";
            httpResponse.Headers["Pragma"] = "no-cache";
            httpResponse.Headers["Expires"] = "-1";
            httpResponse.Headers.Remove("ETag");
        }

        /// <summary>
        ///  初始化全局授权信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sysInfo"></param>
        protected static void InitialAuthorizeInfoFromNew(HttpContext context, AppAuthorizeInfo sysInfo)
        {
            var req = context.Request;

            sysInfo.AppCode = GetInfoValue("Z-AppCode", req);
            if (string.IsNullOrEmpty(sysInfo.AppCode))
                return;

            sysInfo.AppVersion = GetInfoValue("Z-AppVersion", req);
            sysInfo.CustomerId = GetInfoValue("Z-CustomerId", req);
            sysInfo.DeviceId = GetInfoValue("Z-DeviceId", req);
            sysInfo.EmpCode = GetInfoValue("Z-EmpCode", req);
            sysInfo.Ext = GetInfoValue("Z-Ext", req);
            sysInfo.Token = GetInfoValue("Authorization", req);

            sysInfo.IpAddress = GetInfoValue("Z-IpAddress", req);
            sysInfo.LogSerialNum = GetInfoValue("Z-LogSerialNumber", req);
            sysInfo.TimeSpan = GetInfoValue("Z-Timespan", req).ToInt64();
            sysInfo.UserId = GetInfoValue("Z-UserId", req);

            sysInfo.Sign = GetInfoValue("Z-Sign", req);

            var logNum = sysInfo.LogSerialNum;
            if (string.IsNullOrEmpty(sysInfo.LogSerialNum))
            {
                //  还未签名校验，不能赋值给 sysInfo
                logNum = Guid.NewGuid().ToString();
            }
            context.TraceIdentifier = logNum;
        }

        private static string GetInfoValue(string key, HttpRequest req)
        {
            return req.Query.ContainsKey(key) ? req.Query[key].ToString().UrlDecode() : req.Headers[key].ToString();
        }
    }
}
