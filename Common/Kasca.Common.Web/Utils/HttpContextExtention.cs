using Microsoft.AspNetCore.Http;

namespace Kasca.Common.Web.Utils
{
     /// <summary>
     ///  HttpContext 扩展
     /// </summary>
    public static class HttpContextExtention
    {
        /// <summary>
        ///  是否是ajax请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsAjax(this HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
