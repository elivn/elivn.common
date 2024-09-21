using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Kasca.Common.Authrization;

namespace Kasca.Common.Web.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonOutputWithLogFormatter: JsonOutputFormatter
    {
        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="serializerSettings"></param>
        /// <param name="charPool"></param>
        public JsonOutputWithLogFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool):base(serializerSettings,charPool)
        {
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            await  base.WriteResponseBodyAsync(context, selectedEncoding);

            HttpLogUtil.WriteHttpRespLog(context.HttpContext,context.Object,MemberShiper.AppAuthorize?.LogSerialNum);
        }


       
    }
}
