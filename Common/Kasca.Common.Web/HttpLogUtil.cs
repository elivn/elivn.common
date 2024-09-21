using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Kasca.Common.Authrization;
using Kasca.Common.ComUtils;
using Kasca.Common.Plugs;
using Kasca.Common.Plugs.LogPlug;
using System;

namespace Kasca.Common.Web
{
    /// <summary>
    ///  Http请求的日志辅助类
    /// </summary>
    public class HttpLogUtil
    {

        private static readonly JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        private static readonly string moduleName = string.Concat(ModuleNames.Systems, "ApiUse");
        /// <summary>
        ///  记录请求全局日志
        /// </summary>
        /// <param name="sysInfo"></param>
        /// <param name="httpMothed"></param>
        /// <param name="consumeMilSecs"></param>
        /// <param name="hasError"></param>
        /// <returns></returns>
        public static void WriteHttpGlobalLogs(AppAuthorizeInfo sysInfo, string httpMothed, long consumeMilSecs,
            bool hasError = false)
        {
            if (!AppInfoUtil.OpenHttpLog)
                return;

            var body = new StringBuilder();
            body.Append("{\"LogNum\":").Append("\"").Append(sysInfo.LogSerialNum).Append("\"")
                .Append(",\"LogType\":").Append("\"HttpLife\"")
                .Append(",\"HasError\":").Append("\"").Append(hasError ? "异常" : "正常").Append("\"")
                .Append(",\"HttpMothed\":").Append("\"").Append(httpMothed).Append("\"")
                .Append(",\"ConsumeMilSecs\":").Append("\"").Append(consumeMilSecs).Append("\"")
                .Append(",\"IPAddress\":").Append("\"").Append(sysInfo.IpAddress).Append("\"}");
            
            LogUtil.Info(body.ToString(), "HttpBodyLog", moduleName);
        }

        /// <summary>
        ///  写Api接口的响应日志
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <param name="logNum"></param>
        public static void WriteHttpRespLog(HttpContext context, object content, string logNum)
        {
            if (!AppInfoUtil.OpenHttpLog)
                return;

            var reqStr = new StringBuilder();
            //reqStr.Append("{\"LogNum\":").Append("\"").Append(logNum).Append("\"")
            //    .Append(",\"LogType\":").Append("\"Response\"")
            //    .Append(",\"RespBody\":").Append("\"").Append(JsonConvert.SerializeObject(content, jsonSetting))
            //    .Append("\"}");

            var req = context.Request;
            var globalHeader = req.Headers.Where(kp => kp.Key.StartsWith("Z-"))?.Select(kv => string.Concat(kv.Key, "=", kv.Value));

            reqStr.Append("{\"LogNum\":").Append("\"").Append(logNum).Append("\"")
                //.Append(",\"LogType\":").Append("\"Request\"")
                .Append(",\"ReqUrl\":").Append("\"").Append(req.Scheme).Append("://").Append(req.Host)
                .Append(req.PathBase).Append(req.Path).Append(req.QueryString).Append("\"")
                .Append(",\"ReferUrl\":").Append("\"").Append(req.Headers[HeaderNames.Referer]).Append("\"")
                .Append(",\"SysParas\":").Append("\"").Append(string.Join("&", globalHeader)).Append("\"")
                .Append(",\"ReqBody\":").Append("\"").Append(context.Items["reqest_body"])
                .Append(",\"request_time\":").Append("\"").Append(context.Items["request_time"])
                .Append(",\"interalTime\":").Append("\"").Append(DateTime.Now.Ticks - (long)context.Items["request_time"])
                .Append(",\"RespBody\":").Append("\"").Append(JsonConvert.SerializeObject(content, jsonSetting)).Append("\"}");

            WriteHttpLog(reqStr.ToString());
        }
        /// <summary>
        ///   写Api接口的请求日志
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logNum"></param>
        /// <returns></returns>
        public static async Task WriteHttpReqLog(HttpContext context, string logNum)
        {
            if (!AppInfoUtil.OpenHttpLog)
                return;

            //var req = context.Request;
            var content = await GetReqBody(context);
            //var globalHeader = req.Headers.Where(kp => kp.Key.StartsWith("Z-"));

            //var reqLogStr = new StringBuilder();
            //reqLogStr.Append("{\"LogNum\":").Append("\"").Append(logNum).Append("\"")
            //    .Append(",\"LogType\":").Append("\"Request\"")
            //    .Append(",\"ReqUrl\":").Append("\"").Append(req.Scheme).Append("://").Append(req.Host)
            //    .Append(req.PathBase).Append(req.Path).Append(req.QueryString).Append("\"")
            //    .Append(",\"ReferUrl\":").Append("\"").Append(req.Headers[HeaderNames.Referer]).Append("\"")
            //    .Append(",\"SysParas\":").Append("\"").Append(JsonConvert.SerializeObject(globalHeader, jsonSetting))
            //    .Append("\"")
            //    .Append(",\"ReqBody\":").Append("\"").Append(content).Append("\"}");

            //WriteHttpLog(reqLogStr.ToString());

            context.Items.Add("reqest_body", content);
            context.Items.Add("reqTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            context.Items.Add("request_time", DateTime.Now.Ticks);
        }

        /// <summary>
        ///写日志辅助方法
        /// </summary>
        /// <param name="info"></param>
        private static void WriteHttpLog(string info)
        {
            LogUtil.Info(info, "HttpLifeLog", moduleName);
        }


        /// <summary>
        /// 获取请求内容体
        /// </summary>
        /// <param name="context"></param>
        /// <returns> 返回 请求内容 消息</returns>
        private static async Task<string> GetReqBody(HttpContext context)
        {
            var hasBody = context.Request.Method != "GET"
                          && (context.Request.ContentType?.Contains("application/json") ?? false);

            if (!hasBody)
                return string.Empty;

            context.Request.EnableRewind();
         
            return await GetContentFromStream(context.Request.Body);
        }

        private static async Task<string> GetContentFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
            {
                var strContent = await reader.ReadToEndAsync();
                stream.Seek(0, SeekOrigin.Begin);
                return strContent;
            }
        }
    }
}
