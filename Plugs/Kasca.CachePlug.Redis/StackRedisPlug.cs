using System;
using Newtonsoft.Json;
using Kasca.Common.Plugs.CachePlug;
using Kasca.Common.ComUtils;
using System.Threading.Tasks;
using System.Text;
using FreeRedis;

namespace Kasca.CachePlug.Redis
{
    /// <summary>
    /// redis缓存实现类
    /// </summary>
    public class StackRedisPlug : ICachePlug
    {
        //redis数据库连接字符串
        private readonly static string ConnectionStr = null;
        private readonly static string EndPointsStr = null;
        private readonly static string PasswordStr = null;
        // "mycache.redis.cache.windows.net,abortConnect=false, ssl=true,password=..."


        private readonly static int _db = 0;


        private static RedisClient _CacheRedis;

        /// <summary>
        /// 构造函数
        /// </summary>
        static StackRedisPlug()
        {
          //  _CacheRedis = new RedisClient(ConfigUtil.Configuration.GetSection("Redis:RedisConnectionString").Value);
        }


        #region 单例模块

        private static object _lockObj = new object();

        private static StackRedisPlug _instance;

        /// <summary>
        ///   接口请求实例  
        ///  当 DefaultConfig 设值之后，可以直接通过当前对象调用
        /// </summary>
        public static StackRedisPlug Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_lockObj)
                {
                    if (_instance == null)
                        _instance = new StackRedisPlug();
                }

                return _instance;
            }

        }

        #endregion

        /// <summary>
        /// 缓存数据库
        /// </summary>
        // public static RedisClient CacheRedis = new RedisClient(ConfigUtil.Configuration.GetSection("Redis:RedisConnectionString").Value);
        public static RedisClient CacheRedis = new RedisClient(new ConnectionStringBuilder[] { ConfigUtil.Configuration.GetSection("Redis:RedisConnectionString1").Value, ConfigUtil.Configuration.GetSection("Redis:RedisConnectionString2").Value, ConfigUtil.Configuration.GetSection("Redis:RedisConnectionString3").Value });

        /// <summary>
        /// 添加缓存，已存在不更新
        /// </summary>
        /// <typeparam name="T">添加缓存对象类型</typeparam>
        /// <param name="key">添加对象的key</param>
        /// <param name="obj">值</param>
        /// <param name="slidingExpiration">缓存时间 （redis目前都用绝对的）</param>
        /// <param name="absoluteExpiration"> 绝对过期时间（此字段无用 redis目前都用绝对的） </param>
        /// <returns>是否添加成功</returns>
        public bool Add<T>(string key, T obj, TimeSpan slidingExpiration, DateTime? absoluteExpiration)
        {
            if (slidingExpiration == TimeSpan.Zero && absoluteExpiration == null)
                throw new ArgumentNullException(nameof(slidingExpiration), "缓存过期时间不正确,需要设置固定过期时间或者相对过期时间");

            if (obj == null)
                return false;

            var jsonStr = JsonConvert.SerializeObject(obj);

            if (slidingExpiration == TimeSpan.Zero)
            {
                slidingExpiration = new TimeSpan(Convert.ToDateTime(absoluteExpiration).Ticks) - new TimeSpan(DateTime.Now.Ticks);
            }

            CacheRedis.Set(key, jsonStr, (Int32)(slidingExpiration.TotalSeconds / 1));
            return true;
        }

        /// <summary>
        /// 获取缓存数据【同时添加缓存击穿保护】，如果没有则添加
        /// </summary>
        /// <typeparam name="RType"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="getFunc">没有数据时，通过此方法获取原始数据</param>
        /// <param name="absoluteExpiration">固定过期时长，设置后到时过期</param>
        /// <param name="hitProtectCondition">缓存击穿保护触发条件</param>
        /// <param name="hitProtectSeconds">缓存击穿保护秒数</param>
        /// <returns></returns>
        public static  RType GetOrSet<RType>(string cacheKey, Func<Task<RType>> getFunc, TimeSpan absoluteExpiration,
            Func<RType, bool> hitProtectCondition, int hitProtectSeconds)
        {
            if (getFunc == null)
                throw new ArgumentNullException("获取原始数据方法(getFunc)不能为空!");
            if (CacheRedis.Exists(cacheKey))
            {
                var obj = StackRedisPlug.Instance.Get<RType>(cacheKey);
                if (obj != null)
                    return obj;
            }

            var data =  getFunc().Result;

            var hitTrigger = hitProtectCondition?.Invoke(data) ?? data == null;
            if (hitTrigger)
            {
                absoluteExpiration = TimeSpan.FromSeconds(hitProtectSeconds);
            }

            var cacheData = data;
            CacheRedis.Set(cacheKey, cacheData, (Int32)(absoluteExpiration.TotalSeconds/1));

            return data;
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            string value = CacheRedis.Get(key);
            if (string.IsNullOrEmpty(value))
                return default(T);

            try
            {
                T result = JsonConvert.DeserializeObject<T>(value);
                return result;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return CacheRedis.Exists(key);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            CacheRedis.Del(key);
            return true;
        }

        public bool Set<T>(string key, T obj, TimeSpan slidingExpiration)
        {
            return Add(key, obj, slidingExpiration, null);
        }

        public bool Set<T>(string key, T obj, DateTime absoluteExpiration)
        {
            return Add(key, obj, TimeSpan.Zero, absoluteExpiration);
        }

    }
}
