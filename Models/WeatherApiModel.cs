using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.Models
{
    public class WeatherApiModel
    {
        public string cityid { get; set; }

        public string city { get; set; }

        public List<DataModel> data { get; set; }
    }

    public class DataModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// 天气状态
        /// </summary>
        public string wea { get; set; }

        /// <summary>
        /// 白天温度
        /// </summary>
        public string tem1 { get; set; }

        /// <summary>
        /// 晚上温度
        /// </summary>
        public string tem2 { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        public string[] win { get; set; }

        public string wind => string.Join("~", win);

        /// <summary>
        /// 风力
        /// </summary>
        public string win_speed { get; set; }

        public string winspeed => win_speed.Replace("转", "~");
    }
}
