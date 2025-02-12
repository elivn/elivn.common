﻿/*
 * Copyright (C) Alibaba Cloud Computing
 * All rights reserved.
 * 
 * 版权所有 （C）阿里云计算有限公司
 */

using System;

namespace Elivn.AliyunOssSdk.Api.Object.Copy
{
    /// <summary>
    /// 拷贝Object的请求结果。
    /// </summary>
    public class CopyObjectResult
    {
        /// <summary>
        /// 获取新Object最后更新时间。
        /// </summary>
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// 获取新Object的ETag值。
        /// </summary>
        public string ETag { get; set; }
        
        internal CopyObjectResult()
        { }
    }
}
