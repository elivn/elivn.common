#region Copyright (C) 2016 Elivn  

/***************************************************************************
*　　	文件功能描述：通用结果枚举
*
*　　	创建人： Elivn
*       创建人Email：498353921@qq.com
*       
*****************************************************************************/

#endregion

using Kasca.Common.Extention;

namespace Kasca.Common.ComModels.Enums
{
    /// <summary>
    ///   结果类型
    /// </summary>
    public enum ResultTypes
    {
        /// <summary>
        /// 成功
        /// </summary>
        [OSDescript("成功")] Success = 100000,

        /// <summary>
        ///  签名错误
        /// </summary>
        [OSDescript("签名错误")] SignError = 100300,

        /// <summary>
        /// 参数错误
        /// </summary>
        [OSDescript("参数错误")] ParaError = 100301,

        /// <summary>
        /// 添加失败
        /// </summary>
        [OSDescript("添加失败")] AddFail = 100320,

        /// <summary>
        /// 更新失败
        /// </summary>
        [OSDescript("更新失败")] UpdateFail = 100330,

        /// <summary>
        /// 对象不存在
        /// </summary>
        [OSDescript("对象不存在")] ObjectNull = 100404,

        /// <summary>
        /// 对象已存在
        /// </summary>
        [OSDescript("对象已存在")] ObjectExsit = 100410,

        /// <summary>
        /// 对象状态不正常
        /// </summary>
        [OSDescript("对象状态不正常")] ObjectStateError = 100420,

        /// <summary>
        /// 当前状态不允许操作
        /// </summary>
        [OSDescript("当前状态不允许操作")] OperateStateError = 100421,

        /// <summary>
        ///  未知操作
        /// </summary>
        [OSDescript("未知操作")] UnKnowOperate = 100422,

        /// <summary>
        ///  未知来源
        /// </summary>
        [OSDescript("未知来源")] UnKnowSource = 100423,

        /// <summary>
        /// 未登录
        /// </summary>
        [OSDescript("未登录")] UnAuthorize = 100425,

        /// <summary>
        /// 权限不足
        /// </summary>
        [OSDescript("权限不足")] NoRight = 100430,

        /// <summary>
        /// 账号/权限冻结
        /// </summary>
        [OSDescript("账号/权限冻结")]
        AuthFreezed = 100440,

        /// <summary>
        /// 内部错误（服务器错误）
        /// </summary>
        [OSDescript("内部错误")] InnerError = 100500,


        /// <summary>
        ///  网络请求失败
        /// </summary>
        [OSDescript("网络请求失败")] InternetError = 100600,

    }
}
