using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels;
using Kasca.Common.ComModels.Enums;
using Kasca.Common.Http;
using Kasca.Common.Http.Extention;
using Kasca.Common.Http.Mos;
using Kasca.Common.Plugs.LogPlug;

namespace Kasca.Common.ComUtils
{
    /// <summary>
    ///   api 请求辅助类
    /// </summary>
    public static class ApiUtil
    {

        /// <summary>
        ///     put一个Api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <param name="req"></param>
        /// <returns>返回继承 ResultMo 的实体 </returns>
        public static async Task<TRes> PutApi<TRes>(string apiUrl, object req = null, HttpClient client = null)
            where TRes : ResultMo, new()
        {
            return await Rest<TRes>(apiUrl, req, HttpMethod.Put, client);
        }

        /// <summary>
        ///     post一个Api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <param name="req"></param>
        /// <returns>返回继承 ResultMo 的实体 </returns>
        public static async Task<TRes> PostApi<TRes>(string apiUrl, object req = null, HttpClient client = null)
            where TRes : ResultMo, new()
        {
            return await Rest<TRes>(apiUrl, req, HttpMethod.Post,client);
        }

        /// <summary>
        ///   post一个api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <param name="req"></param>
        /// <returns>返回  ResultMo &lt;TRes&gt; 类型的结果实体</returns>
        public static async Task<ResultMo<TRes>> PostEntApi<TRes>(string apiUrl, object req = null, HttpClient client = null)
        {
            return await Rest<ResultMo<TRes>>(apiUrl, req, HttpMethod.Post,client);
        }



        /// <summary>
        /// get 一个api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <returns>返回继承 ResultMo 的实体 </returns>
        public static async Task<TRes> GetApi<TRes>(string apiUrl, HttpClient client = null)
            where TRes : ResultMo, new()
        {
            return await Rest<TRes>(apiUrl, null, HttpMethod.Get,  client);
        }
        
        /// <summary>
        /// get 一个api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        ///  <returns>返回  ResultMo &lt;TRes&gt; 类型的结果实体</returns>
        public static async Task<ResultMo<TRes>> GetEntApi<TRes>(string apiUrl, HttpClient client=null)
        {
            return await Rest<ResultMo<TRes>>(apiUrl, null, HttpMethod.Get, client);
        }
        
        private static async Task<TRes> Rest<TRes>(string absoluateApiUrl, object reqContent, HttpMethod mothed,
            HttpClient client)
            where TRes : ResultMo, new()
        {
            try
            {
                var content = reqContent == null
                    ? null
                    : (reqContent is string
                        ? reqContent.ToString()
                        : JsonConvert.SerializeObject(reqContent, Formatting.None, new JsonSerializerSettings
                        {
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore
                        }));

                var httpReq = new OsHttpRequest
                {
                    HttpMethod = mothed,
                    AddressUrl = absoluateApiUrl,
                    CustomBody = content,
                    RequestSet = SetReqestFormat
                };
                return await httpReq.RestCommonJson<TRes>(client);
            }
            catch (Exception error)
            {
#if DEBUG
                throw error;
#endif
                LogUtil.Error(string.Concat("错误信息：", error.Message, "详细信息：", error.StackTrace), "RestApi");
            }

            return new TRes() {code = (int) ResultTypes.InnerError, msg = "应用接口响应失败！"};
        }

        /// <summary>
        ///     post一个Api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <param name="req"></param>
        /// <returns>返回继承 ResultMo 的实体 </returns>
        public static async Task<string> PostApi(string apiUrl, string req = null, HttpClient client = null)
        {
            return await RestStr(apiUrl, req, HttpMethod.Post, client);
        }
        /// <summary>
        /// get 一个api请求
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="apiUrl"></param>
        /// <returns>返回继承 ResultMo 的实体 </returns>
        public static async Task<string> GetApi(string apiUrl, HttpClient client = null)
        {
            return await RestStr(apiUrl, null, HttpMethod.Get, client);
        }

        /// <summary>
        ///  使用json格式化内容方法
        /// </summary>
        /// <typeparam name="TResp"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private static async Task<TResp> JsonFormat<TResp>(HttpResponseMessage resp)
            where TResp : ResultMo, new()
        {
            if (!resp.IsSuccessStatusCode)
                return new TResp()
                {
                    code = (int)ResultTypes.InternetError,
                    msg = resp.ReasonPhrase
                };

            string contentStr;
            using (resp)
            {
                contentStr = await resp.Content.ReadAsStringAsync();
            }
            return JsonConvert.DeserializeObject<TResp>(contentStr);
        }



        /// <summary>
        ///   上传文件处理
        /// </summary>
        /// <param name="absoluateApiUrl"></param>
        /// <param name="files"></param>
        /// <param name="forms"></param>
        /// <returns></returns>
        public static async Task<string> PostApiWithFile(string absoluateApiUrl, List<FileParameter> files,
            List<FormParameter> forms = null, HttpClient client = null)
        {
            try
            {
                var httpReq = new OsHttpRequest
                {
                    HttpMethod = HttpMethod.Post,
                    AddressUrl = absoluateApiUrl,
                    FileParameters = files,
                    RequestSet = SetReqestFormat
                };
                if (forms != null)
                    httpReq.FormParameters = forms ;
                
                var resp = await httpReq.RestSend(client);
                return await StrFormat(resp);
            }
            catch (Exception error)
            {
#if DEBUG
                throw error;
#endif
                LogUtil.Error(string.Concat("错误信息：", error.Message, "详细信息：", error.StackTrace), "RestApi");
            }

            return $"{{\"code\":{(int)ResultTypes.InnerError},\"msg\":\"应用接口响应失败！\"}}";
        }

        private static void SetReqestFormat(HttpRequestMessage r)
        {
            var sysInfo = MemberShiper.AppAuthorize??new AppAuthorizeInfo();

            var headerDics = sysInfo.CompleteDicSign(AppInfoUtil.AppCode, AppInfoUtil.AppVersion, 
                AppInfoUtil.AppSecret);
            if (headerDics != null)
            {
                foreach (var item in headerDics)
                {
                    r.Headers.Add(item.Key, item.Value);
                }
            }
           
            r.Headers.Add("Accept", "application/json");
            if (r.Content != null)
            {
               // r.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sysInfo.Token.Replace("Bearer ", ""));
                r.Content.Headers.ContentType =
                   new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };
            }
               
        }
        
        /// <summary>
        ///  和实体类型无关的接口操作【字符串请求和返回都是字符串】
        /// </summary>
        /// <param name="absoluateApiUrl"></param>
        /// <param name="reqContent"></param>
        /// <param name="mothed"></param>
        /// <returns></returns>
        public static async Task<string> RestStr(string absoluateApiUrl, string reqContent, HttpMethod mothed, HttpClient client = null)
        {
            try
            {
                var httpReq = new OsHttpRequest
                {
                    HttpMethod = mothed,
                    AddressUrl = absoluateApiUrl,
                    CustomBody = reqContent,
                    RequestSet = SetReqestFormat
                };
                var resp = await httpReq.RestSend(client);

                return await StrFormat(resp);
            }
            catch (Exception error)
            {
#if DEBUG
                throw error;
#endif
                LogUtil.Error(string.Concat("错误信息：", error.Message, "详细信息：", error.StackTrace), "RestApi");
            }
            return $"{{\"code\":{(int)ResultTypes.InnerError},\"msg\":\"应用接口响应失败！\"}}";
        }
        
        private static async Task<string> StrFormat(HttpResponseMessage resp)
        {
            if (!resp.IsSuccessStatusCode)
                return $"{{\"code\":{(int) ResultTypes.InternetError},\"msg\":\"{resp.ReasonPhrase}\"}}";

            string resStr;
            using (resp)
            {
                resStr = await resp.Content.ReadAsStringAsync();
            }
            return resStr;
        }

    }
}
