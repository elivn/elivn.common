#region Copyright (C) 2016 Elivn  

/***************************************************************************
*　　	文件功能描述：通用系统授权信息
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       
*****************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Kasca.Common.Encrypt;
using Kasca.Common.Extention;

namespace Kasca.Common.Authrization
{
    /// <summary>
    ///   应用的授权认证信息
    /// </summary>
    public class AppAuthorizeInfo
    {
        #region  参与签名属性

        /// <summary>
        ///   应用来源
        /// </summary>
        public string AppCode { get; set; }

        /// <summary>
        ///   应用版本
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        ///  客户Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmpCode { get; set; }
        
 


        /// <summary>
        /// IP地址 可选 手机App为空
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///  跟踪代码 
        /// </summary>
        public string LogSerialNum { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public long TimeSpan { get; set; }

        /// <summary>
        ///  会员Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///  签名信息
        /// </summary>
        public string Sign { get; set; }

        #endregion
        
        #region 非签名字段
        
        /// <summary>
        /// 扩展字段信息
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        #endregion

        #region  签名相关

        /// <summary>
        ///   获取要加密签名的串
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetToHeaders(string appCode, string appVersion)
        {
            var headers = new Dictionary<string, string>();

            Addheaders(headers, "Z-AppCode", appCode);
            Addheaders(headers, "Z-AppVersion", appVersion);
            Addheaders(headers, "Z-CustomerId", CustomerId);
            Addheaders(headers, "Z-DeviceId", DeviceId);
            Addheaders(headers, "Z-EmpCode", EmpCode);
   
            Addheaders(headers, "Z-IpAddress", IpAddress);
            Addheaders(headers, "Z-LogSerialNumber", LogSerialNum);
            Addheaders(headers, "Z-Timespan", TimeSpan.ToString());
            Addheaders(headers, "Z-UserId", UserId);

            return headers;
        }

        private static void Addheaders(Dictionary<string, string> headers,string key,string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                headers.Add(key,val);
            }
        }

        /// <summary>
        /// 补全签名信息
        /// </summary>
        /// <param name="appCode"></param>
        /// <param name="appVersion"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public Dictionary<string, string> CompleteDicSign(string appCode, string appVersion,
            string secretKey)
        {
            TimeSpan = DateTime.Now.ToUtcSeconds();
            var headers = GetToHeaders(appCode, appVersion);
            var sign = GenerateSign(headers, secretKey);

          //  Addheaders(headers, "Authorization", Token);
            Addheaders(headers, "Z-Sign", sign);
            Addheaders(headers,"Z-Ext", Ext);

            return headers;
        }

        /// <summary>
        /// 对比当前头信息实体的签名
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public bool CheckSign(string secretKey)
        {
            var headers = GetToHeaders(AppCode, AppVersion);
            var signData = GenerateSign(headers, secretKey);

            return Sign == signData;
        }

        #endregion

        private string GenerateSign(Dictionary<string, string> headers,  string secretKey)
        {
            var strEncry = new StringBuilder();
            foreach (var h in headers)
            {
                strEncry.Append(h.Key).Append("=").Append(h.Value).Append("&");
            }
            return HMACSHA.EncryptBase64(strEncry.ToString().TrimEnd('&'), secretKey);
        }

    }
}