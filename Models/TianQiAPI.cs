using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.Models
{
    public class TianQiApi
    {
        public string city { get; set; }

        public IList<WeatherInfo> tqInfo { get; set; }
    }

    public class WeatherInfo
    {
        public string ymd { get; set; }

        public string bWendu { get; set; }

        public string yWendu { get; set; }

        public string tianqi { get; set; }

        public string fengxiang { get; set; }

        public string fengli { get; set; }
    }
}
