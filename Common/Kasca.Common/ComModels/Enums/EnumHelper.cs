using Kasca.Common.Extention;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Kasca.Common.ComModels.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 任务规则key码
        /// </summary>
        public enum TaskKeyEnum
        {
            /// <summary>
            /// 每日签到
            /// </summary>
            [OSDescript("每日签到")]
            SignIn,

            /// <summary>
            /// 阅读资讯
            /// </summary>
            [OSDescript("阅读资讯")]
            ReadNews,

            /// <summary>
            /// 妈妈分享回帖
            /// </summary>
            [OSDescript("妈妈分享回帖")]
            RreplyParentShare,

            /// <summary>
            /// 发布帖子
            /// </summary>
            [OSDescript("发布帖子")]
            CreatePosts,

            /// <summary>
            /// 在线聊天
            /// </summary>
            [OSDescript("在线聊天")]
            OnlineChat,

            /// <summary>
            /// 观看直播.
            /// </summary>
            [OSDescript("观看直播")]
            WatchLive,

            /// <summary>
            /// 邀请有礼
            /// </summary>
            [OSDescript("邀请有礼")]
            Invite,

            /// <summary>
            /// 分享知识
            /// </summary>
            [OSDescript("分享知识")]
            ShareKnowledge,

            /// <summary>
            /// 新会员注册
            /// </summary>
            [OSDescript("新会员注册")]
            MemberRegist
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            try
            {
                string value = enumValue.ToString();
                FieldInfo field = enumValue.GetType().GetField(value);
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
                if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                    return value;
                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
                return descriptionAttribute.Description;
            }
            catch
            {
                return "";
            }

        }
    }
}
