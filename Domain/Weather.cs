﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Weather.Domain
{
    public class WeatherData
    {
        public int Id { get; set; }

        public string City { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 天气状况
        /// </summary>
        public string WeatherType { get; set; }

        /// <summary>
        /// 白天气温
        /// </summary>
        public string DayTemperature { get; set; }

        /// <summary>
        /// 夜间气温
        /// </summary>
        public string NightTemperature { get; set; }

        /// <summary>
        /// 风力类型
        /// </summary>
        public string WindType { get; set; }

        /// <summary>
        /// 风力大小
        /// </summary>
        public string WindSpeed { get; set; }

        //public WeatherType WeatherTypes
        //{
        //    get => (WeatherType)WeatherTypeId;
        //    set => WeatherTypeId = (int)value;
        //}

        [Flags]
        public enum WeatherStatusType
        {
            晴,
            阴,
            多云,
            雨夹雪,
            小雨,
            中雨,
            阵雨,
            小雪,
            中雪,
            大雪,
            大雨,
            雾,
            暴雨,
            雷阵雨,
            阵雪,
            暴雪,
            扬沙,
            大暴雨,
            霾,
            浮尘,
            晴转多云,
            小雪转晴,
            多云转晴,
            多云转阴,
            晴转阴,
            阴转多云,
            多云转小雪,
            阵雪转晴,
            晴转阵雪,
            小雪转多云,
            小雨转多云,
            晴转小雪,
            多云转雨夹雪,
            多云转阵雪,
            阵雨转多云,
            多云转小雨,
            多云转阵雨,
            阵雪转小雪,
            阴转小雪,
            小雪转阴,
            阵雪转多云,
            阴转晴,
            阴转阵雪,
            阵雪转阴,
            扬沙转多云,
            扬沙转晴,
            浮尘转晴,
            晴转雨夹雪,
            多云转中雪,
            晴转中雪,
            阴转小雨,
            小雨转中雨,
            小雨转阴,
            中雨转多云,
            中雨转小雨,
            阴转中雨,
            多云转中雨,
            小雨转大雨,
            阵雨转中雨,
            阵雨转大雨,
            阴转大雨,
            雾转多云,
            阵雨转小雨,
            中雨转阴,
            晴转小雨,
            多云转大雨,
            小雨转暴雨,
            阵雨转晴,
            小雨转晴,
            阵雨转中到大雨,
            小雨转阵雨,
            阵雨转阴,
            雨夹雪转晴,
            雨夹雪转多云,
            小雨转小雪,
            小雪转雨夹雪,
            阴转阵雨,
            小雨转小到中雨,
            小到中雨转小雨,
            小到中雨转阴,
            晴转阵雨,
            中雨转阵雨,
            阵雨转雷阵雨,
            多云转大雪,
            阴转中雪,
            阴转大雪,
            雨夹雪转阴,
            雨夹雪转小雪,
            小雨转大雪,
            雨夹雪转大雪,
            雨夹雪转中雪,
            中雨转小雪,
            中雨转中雪,
            晴转大雪,
            小雨转雨夹雪,
            阴转雨夹雪,
            多云转雾,
            小雪转阵雪,
            小雪转中雪,
            多云转小到中雪,
            中雪转多云,
            中雪转小雪,
            大雪转小雪,
            中雨转大雨,
            阵雨转雨夹雪,
            多云转小到中雨,
            小到中雨,
            小到中雨转阵雨,
            小雨转阵雪,
            雷阵雨转多云,
            雷阵雨转阵雨,
            多云转扬沙,
            晴转扬沙,
            扬沙转阴,
            浮尘转霾,
            晴转霾,
            霾转阴,
            霾转多云,
            霾转晴,
            小到中雪转多云,
            大雪转多云,
            雨夹雪转小雨,
            大雨转阴,
            浮尘转多云,
            多云转霾,
            晴转雾,
            小雨转中雪,
            阵雨转小雪,
            晴转雷阵雨,
            阴转雾
        }
    }
}
