using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kasca.Common.Authrization;
using Kasca.Common.ComUtils;

namespace Kasca.Common.Web.Filters
{
    public class CallApiAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        ///  获取IP地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetIpAddress(HttpContext context)
        {
            string ipAddress = context.Request.Headers["X-Forwarded-For"];
            return !string.IsNullOrEmpty(ipAddress) ? ipAddress : context.Connection.RemoteIpAddress.ToString();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var sysInfo = MemberShiper.AppAuthorize;

            if (sysInfo==null)
            {
                sysInfo= new AppAuthorizeInfo();
                MemberShiper.SetAppAuthrizeInfo(sysInfo);
            }

            SetSystemAuthorizeInfo(sysInfo, context);
        }


        private static void SetSystemAuthorizeInfo(AppAuthorizeInfo sysInfo, ActionContext context)
        {
            if (string.IsNullOrEmpty(sysInfo.IpAddress))
                sysInfo.IpAddress = GetIpAddress(context.HttpContext);

            sysInfo.AppCode = AppInfoUtil.AppCode;
            sysInfo.AppVersion = AppInfoUtil.AppVersion;

            if (string.IsNullOrEmpty(sysInfo.LogSerialNum))
            {
                sysInfo.LogSerialNum = context.HttpContext.TraceIdentifier;
            }
        }
    }
}
