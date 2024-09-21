using System;
using System.Collections.Concurrent;

namespace Kasca.Common.RepeatCheck
{
    /// <summary>
    /// 单机版重复校验
    /// </summary>
    public static class RepeatCheckUtil
    {
        private static ConcurrentDictionary<string, int> lockDics = new ConcurrentDictionary<string, int>();

        /// <summary>
        ///  
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryEnter(string key)
        {
            return lockDics.TryAdd(key, 1);
        }

        public static void Quit(string key)
        {
            lockDics.TryRemove(key, out int temp);
        }

        public static RepeatCheckLife GetCheckLife( string key)
        {
            return TryEnter(key) 
                ? new RepeatCheckLife(key,true) 
                : new RepeatCheckLife();
        }
    }

    /// <summary>
    ///  重复检查活动存活类
    /// </summary>
    public class RepeatCheckLife : IDisposable
    {
        /// <summary>
        /// check关键字
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        ///  是否已经激活
        /// </summary>
        public bool IsActived { get; private set; }

        /// <inheritdoc />
        public RepeatCheckLife()
        {
            IsActived = false;
        }

        /// <inheritdoc />
        public RepeatCheckLife(string key,bool isActived)
        {
            Key = key;
            IsActived = isActived;
        }


        /// <inheritdoc />
        public void Dispose()
        {
            if (IsActived)
            {
                RepeatCheckUtil.Quit(Key);
            }
        }
    }
}
