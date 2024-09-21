#region Copyright (C) 2016 Elivn  

/***************************************************************************
*　　	文件功能描述：OSS.Http.Extention - 通用App接口请求基类
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       创建时间： 2017-5-25
*       
*****************************************************************************/

#endregion

using System;
using System.Threading;
using Kasca.Common.ComModels;
using Kasca.Common.Plugs;

namespace Kasca.Common.Http
{
    /// <summary>
    ///   请求基类
    /// </summary>
    /// <typeparam name="RestType"></typeparam>
    public class BaseRestApi<RestType> : BaseRestApi<RestType, AppConfig>
        where RestType : class, new()
    {
        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="config"></param>
        public BaseRestApi(AppConfig config = null) : base(config)
        {

        }
    }

    /// <summary>
    /// 通用App接口请求基类
    /// </summary>
    /// <typeparam name="RestType"></typeparam>
    /// <typeparam name="TConfigType"></typeparam>
    public class BaseRestApi<RestType, TConfigType>
        where RestType : class, new()
        where TConfigType : class, new()
    {

        #region  接口配置信息

        /// <summary>
        ///   默认配置信息，如果实例中的配置为空会使用当前配置信息
        /// </summary>
        public static TConfigType DefaultConfig { get; set; }

        private static readonly AsyncLocal<TConfigType> _contextConfig=new AsyncLocal<TConfigType>();
            private readonly TConfigType _config;

        /// <summary>
        ///  设置上下文配置信息，当前配置在当前上下文中有效
        /// </summary>
        /// <param name="config"></param>
        public static void SetContextConfig(TConfigType config)
        {
            _contextConfig.Value = config;
        }

        /// <summary>
        /// 微信接口配置
        /// </summary>
        public TConfigType ApiConfig
        {
            get
            {
                if (_config!=null)
                {
                    return _config;
                }

                if (_contextConfig.Value!=null)
                {
                    return _contextConfig.Value;
                }

                if (DefaultConfig!=null)
                {
                    return DefaultConfig;
                }
                throw new ArgumentNullException("当前配置信息为空，请通过SetContextConfig方法设置,或者构造函数中赋值，或者也可以程序入口处给 DefaultConfig 赋值");
            }
        }
       
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public BaseRestApi(TConfigType config = null)
        {
            _config = config;
        }

        #endregion

        #region  单例实体

        private static object _lockObj = new object();

        private static RestType _instance;

        /// <summary>
        ///   接口请求实例  
        ///  当 DefaultConfig 设值之后，可以直接通过当前对象调用
        /// </summary>
        public static RestType Instance
        {
            get
            {
                if (_instance != null) return _instance;

                lock (_lockObj)
                {
                    if (_instance == null)
                        _instance = new RestType();
                }
                return _instance;
            }

        }

        #endregion

        /// <summary>
        ///   当前模块名称
        ///     方便日志追踪
        /// </summary>
        protected static string ModuleName { get; set; } = ModuleNames.Default;
    }
}
