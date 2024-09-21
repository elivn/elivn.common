#region Copyright (C) 2020 OS系列开源项目

/*       
　　	文件功能描述：验证属性attribute

　　	创建人：李文
        创建人Email：498353921@qq.com
    	创建日期：2020.11.25

　　	修改描述：
*/

#endregion
using System;

namespace Kasca.Common.Extention
{
    /// <summary>
    /// 时间秒数转化
    /// </summary>
    public static class DateTimeExtention
    {
        private static readonly long startTicks = new DateTime(1970, 1, 1).Ticks;

        /// <summary>
        /// 系统定义时间最小值
        /// </summary>
        public static DateTime DateTimeMinValue = new DateTime(1900, 01, 01, 0, 0, 0);

        /// <summary>
        /// 获取距离 1970-01-01（格林威治时间）的秒数
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static long ToUtcSeconds(this DateTime localTime)
        {
            return (localTime.ToUniversalTime().Ticks - startTicks)/10000000;
        }

        /// <summary>
        /// 距离 1970-01-01（格林威治时间）的秒数转换为当前时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromUtcSeconds(this long seconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;// new DateTime(1970, 1, 1).AddSeconds(seconds).ToLocalTime();
        }


        /// <summary>
        /// 获取距离 1970-01-01（格林威治时间）的秒数
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static long ToUtcMilliSeconds(this DateTime localTime)
        {
            return (localTime.ToUniversalTime().Ticks - startTicks) / 10000;
        }

        /// <summary>
        /// 距离 1970-01-01（格林威治时间）的秒数转换为当前时间
        /// </summary>
        /// <param name="milliSeconds"></param>
        /// <returns></returns>
        public static DateTime FromUtcMilliSeconds(this long milliSeconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliSeconds).LocalDateTime;
        }

        /// <summary>
        /// 获取距离 1970-01-01（本地/北京时间）的秒数
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static long ToLocalSeconds(this DateTime localTime)
        {
            return (localTime.Ticks - startTicks) / 10000000;
        }

        /// <summary>
        /// 距离 1970-01-01（本地/北京时间）的秒数转换为当前时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromLocalSeconds(this long seconds)
        {
            return new DateTime(1970,1,1).AddSeconds(seconds);
        }


        /// <summary>
        /// 获取时间差（天数）
        /// </summary>
        /// <param name="dateEnd"></param>
        /// <param name="dateStart"></param>
        /// <returns></returns>
        public static int DateDiff(DateTime dateEnd, DateTime dateStart)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }

        /// <summary>
        /// 根据年龄获取年龄
        /// </summary>
        /// <param name="dtBirthday">出生日期</param>
        /// <param name="dtNow">根据时间计算的日期</param>
        /// <param name="yearName">年份的单位，默认为“岁”</param>
        /// <returns></returns>
        public static string GetAge(DateTime dtBirthday, DateTime dtNow, string yearName = "岁")
        {
            string strAge = string.Empty; // 年龄的字符串表示
            int intYear = 0; // 岁
            int intMonth = 0; // 月
            int intDay = 0; // 天

            // 计算天数
            intDay = dtNow.Day - dtBirthday.Day;
            if (intDay < 0)
            {
                dtNow = dtNow.AddMonths(-1);
                intDay += DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            }

            // 计算月数
            intMonth = dtNow.Month - dtBirthday.Month;
            if (intMonth < 0)
            {
                intMonth += 12;
                dtNow = dtNow.AddYears(-1);
            }

            // 计算年数
            intYear = dtNow.Year - dtBirthday.Year;

            // 格式化年龄输出
            if (intYear >= 1) // 年份输出
            {
                strAge = intYear.ToString() + yearName;
            }

            if (intMonth > 0 && intYear <= 5) // 五岁以下可以输出月数
            {
                strAge += intMonth.ToString() + "月";
            }

            if (intDay >= 0 && intYear < 1) // 一岁以下可以输出天数
            {
                if (strAge.Length == 0 || intDay > 0)
                {
                    strAge += intDay.ToString() + "日";
                }
            }

            return strAge;
        }

        /// <summary>
        /// 获取时间差
        /// </summary>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns> hours + "时" + minutes + "分" + seconds + "秒"</returns>
        public static string GetTimeDiff(TimeSpan ts1, TimeSpan ts2)
        {
            string str = "";
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            string hours = ts.Hours.ToString(), minutes = ts.Minutes.ToString(), seconds = ts.Seconds.ToString();

            if (ts.Hours < 10)
            {
                hours = "0" + ts.Hours.ToString();
            }
            if (ts.Minutes < 10)
            {
                minutes = "0" + ts.Minutes.ToString();
            }
            if (ts.Seconds < 10)
            {
                seconds = "0" + ts.Seconds.ToString();
            }

            str = hours + "时" + minutes + "分" + seconds + "秒";

            return str;
        }

    }
}
