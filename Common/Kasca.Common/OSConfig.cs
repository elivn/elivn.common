#region Copyright (C) 2016 Elivn  

/***************************************************************************
*　　	文件功能描述：全局模块配置内
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       
*****************************************************************************/

#endregion

using System;
using Kasca.Common.Plugs.CachePlug;
using Kasca.Common.Plugs.DirConfigPlug;
using Kasca.Common.Plugs.LogPlug;

namespace Kasca.Common
{
    /// <summary>
    /// 基础配置模块
    /// </summary>
    public static class OsConfig
    {
        #region  Module初始化模块

        /// <summary>
        ///   日志模块提供者
        /// </summary>
        public static Func<string, ILogPlug> LogWriterProvider { get; set; }

        /// <summary>
        ///   缓存模块提供者
        /// </summary>
        public static Func<string, ICachePlug> CacheProvider { get; set; }
        
        /// <summary>
        ///   配置信息模块提供者
        /// </summary>
        public static Func<string, IDirConfigPlug> DirConfigProvider { get; set; }
        #endregion


    }
}
