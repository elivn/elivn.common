#region Copyright (C) 2016 Elivn  

/***************************************************************************
*　　	文件功能描述：全局插件 -  日志插件接口
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       
*       
*****************************************************************************/

#endregion

namespace Kasca.Common.Plugs.LogPlug
{
    /// <summary>
    /// 日志写实现接口
    /// </summary>
    public interface ILogPlug
    {
        /// <summary>
        ///   日志写功能
        /// </summary>
        ///<param name="info">日志实体</param>
        void WriteLog(LogInfo info);

        /// <summary>
        ///   设置日志编号
        /// </summary>
        /// <param name="info"></param>
        void SetLogCode(LogInfo info);
    }
}