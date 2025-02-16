#region Copyright (C) 2016 Elivn 

/***************************************************************************
*　　	文件功能描述：全局插件 -  缓存插件接口
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       
*       
*****************************************************************************/

#endregion

using System;

namespace Kasca.Common.Plugs.CachePlug
{
    /// <summary>
    /// 缓存插件接口
    /// </summary>
    public interface ICachePlug
    {
        /// <summary> 
        /// 添加时间段过期缓存
        /// 如果存在则更新
        /// </summary>
        /// <typeparam name="T">添加缓存对象类型</typeparam>
        /// <param name="key">添加对象的key</param>
        /// <param name="obj">值</param>
        /// <param name="slidingExpiration">相对过期的TimeSpan</param>
        /// <returns>是否添加成功</returns>
        bool Set<T>(string key, T obj, TimeSpan slidingExpiration);

        /// <summary>
        /// 添加固定过期时间缓存,如果存在则更新
        /// </summary>
        /// <typeparam name="T">添加缓存对象类型</typeparam>
        /// <param name="key">添加对象的key</param>
        /// <param name="obj">值</param>
        /// <param name="absoluteExpiration"> 绝对过期时间,不为空则按照绝对过期时间计算 </param>
        /// <returns>是否添加成功</returns>
        bool Set<T>(string key, T obj, DateTime absoluteExpiration);

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">获取缓存对象类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>获取指定key对应的值 </returns>
        T Get<T>(string key);

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns>是否成功</returns>
        bool Remove(string key);

    }
}