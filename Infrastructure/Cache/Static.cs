using System;
using System.Collections.Generic;
using System.Text;

namespace Weather.Infrastructure.Cache
{
    public class WeatherStatic
    {
        /// <summary>
        /// 数据缺失日志文件名称
        /// </summary>
        public const string DataLostLog = "datalost";

        /// <summary>
        /// 数据写入日志文件名称
        /// </summary>
        public const string DataSetLog = "datalog";

        #region http://tianqi.2345.com

        //历史数据入口--http://tianqi.2345.com/t/wea_history/js/60005_201112.js
        public const string ApiWeatherUrl1 = "http://tianqi.2345.com/t/wea_history/js/{0}_{1}.js";
        //部分年份地址改变为：--http://tianqi.2345.com/t/wea_history/js/201712/60005_201712.js
        public const string ApiWeatherUrl2 = "http://tianqi.2345.com/t/wea_history/js/{0}/{1}_{2}.js";

        public enum CityType
        {
            Minhang = 60008,
            Baoshan = 71072,
            Jiading = 60010,
            Nanhui = 60299,
            Jinshan = 60006,
            Qingpu = 60007,
            Songjiang = 60009,
            Fengxian = 60298,
            Chongming = 60005,
            Pudong = 71146,
            Xuhui = 71147,
            Huangpu = 71447,
            Changning = 71448,
            Jingan = 71449,
            Putuo = 71450,
            Hongkou = 71451,
            Yangpu = 71452
        }

        #endregion

        #region tianqiAPI

        public const string TianQiApi = "https://www.tianqiapi.com/api/";

        public enum ApiCityType
        {
            Minhang = 101020200,
            Baoshan = 101020300,
            Jiading = 101020500,
            Nanhui = 101020600,
            Jinshan = 101020700,
            Qingpu = 101020800,
            Songjiang = 101020900,
            Fengxian = 101021000,
            Chongming = 101021100,
            Pudong = 101020600,
            Xuhui = 101021200,
            Huangpu = 101280111,
            Changning = 101290505,
            Jingan = 101021400,
            Putuo = 101021500,
            Hongkou = 101021600,
            Yangpu = 101021700
        }

        #endregion

    }
}
