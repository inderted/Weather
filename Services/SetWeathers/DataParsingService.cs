using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Weather.Data;
using Weather.Domain;
using Weather.Infrastructure.Cache;
using Weather.Models;
using Weather.Services.HttpMethods;

namespace Weather.Services.SetWeathers
{
    public class DataParsingService : IDataParsingService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpDataMethodService _httpDataMethodService;

        public DataParsingService(DataContext dataContext,
            IHttpDataMethodService httpDataMethodService)
        {
            _dataContext = dataContext;
            _httpDataMethodService = httpDataMethodService;
        }

        //public void SetWeatherByHouBao(string city, int year, int month)
        //{
        //    var datetime = new DateTime(year, month, 1);
        //    var url = string.Format(WeatherStatic.LSWeatherUrl, WeatherStatic.WeatherType.lishi, city,
        //        datetime.ToString("yyyyMM"));
        //    var doc = _httpDataMethodService.GetHtmlDoc(new Uri(url));
        //    //更加Xpath获取表格对象
        //    var res = doc.DocumentNode.SelectSingleNode(@"//*[@id=""content""]/table");
        //    if (res != null)
        //    {
        //        //获取所有行
        //        var list = res.SelectNodes(@"tr");
        //        list.RemoveAt(0); //移除第一行，是表头
        //        if (list.Count != DateTime.DaysInMonth(year, month) && month != DateTime.Now.Month)
        //        {
        //            _httpDataMethodService.SetLog($"www.tianqihoubao->{city}缺少{year}年{month}月的数据", WeatherStatic.DataSetLog);
        //        }
        //        // 遍历每一行，获取日期，以及天气状况等信息
        //        foreach (var item in list)
        //        {
        //            var dd = item.SelectNodes(@"td");
        //            //日期 -  - 气温 - 风力风向
        //            if (dd.Count != 4) continue;

        //            //获取当前行天气状况
        //            var weatherType = dd[1].InnerText.Replace("\r\n", "").Replace(" ", "").Replace("/", "~").Trim();
        //            //获取当前行气温
        //            var temperature = dd[2].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var temperatures = temperature.Split("/");
        //            //获取当前行风力风向
        //            var wind = dd[3].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var windtype = string.Join("~", Regex.Matches(wind, @"[\u4e00-\u9fa5]{2,3}").Select(x => x.Value));
        //            var windspeed = string.Join("~", Regex.Matches(wind, @"\d-\d[\u4e00-\u9fa5]").Select(x => x.Value));
        //            var weather = new WeatherData()
        //            {
        //                City = city,
        //                //获取当前行日期
        //                Date = DateTime.Parse(dd[0].InnerText.Replace("\r\n", "").Replace(" ", "").Trim()),
        //                WeatherType = weatherType,
        //                DayTemperature = temperatures.FirstOrDefault(),
        //                NightTemperature = temperatures.LastOrDefault(),
        //                WindType = windtype,
        //                WindSpeed = windspeed
        //            };
        //            var contentLog =
        //                $"{city}=>{weather.Date}:{weather.WeatherType},{weather.DayTemperature},{weather.NightTemperature},{weather.WindType},{weather.WindSpeed}";
        //            //打印Log
        //            Console.WriteLine(contentLog);
        //            _httpDataMethodService.SetLog(contentLog, WeatherStatic.DataSetLog);
        //            InsertOrUpdate(weather);
        //        }
        //    }
        //}

        //public void SetWeatherByBaidu(int city, int year, int month)
        //{
        //    var cityName = ((WeatherStatic.BaiduCityType)city).ToString();
        //    var datetime = new DateTime(year, month, 1);
        //    var url = string.Format(WeatherStatic.LSbdWeatherUrl, city, datetime.ToString("yyyyMM"));
        //    var doc = _httpDataMethodService.GetHtmlDoc(new Uri(url));
        //    //Xpath获取表格对象
        //    var res = doc.DocumentNode.SelectSingleNode(@"//*[@class=""history""]/table");
        //    if (res != null)
        //    {
        //        //获取所有行
        //        var list = res.SelectNodes(@"tr");
        //        list.RemoveAt(0); //移除第一行，是表头
        //        if (list.Count != DateTime.DaysInMonth(year, month) && month != DateTime.Now.Month)
        //        {
        //            _httpDataMethodService.SetLog($"www.baidutianqi.com->{cityName}缺少{year}年{month}月的数据", WeatherStatic.DataLostLog);
        //            //Console.WriteLine("数据缺失，尝试切换爬虫入口获取缺失数据");
        //            //SetWeatherByHouBao(cityName.ToLower(), year, month);
        //        }
        //        // 遍历每一行，获取日期，以及天气状况等信息
        //        foreach (var item in list)
        //        {
        //            var dd = item.SelectNodes(@"td");
        //            //日期 -  - 气温 - 风力风向
        //            if (dd.Count != 6) continue;

        //            var datetep = dd[0].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var date = DateTime.Parse(datetep.Substring(0, datetep.IndexOf('（')));

        //            //获取当前行气温
        //            var dayTemperature = dd[1].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var nightTemperature = dd[2].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            //获取当前行天气状况
        //            var weatherType = dd[3].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            //获取当前行风力风向
        //            var windtype = dd[4].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var windspeed = dd[5].InnerText.Replace("\r\n", "").Replace(" ", "").Trim();
        //            var weather = new WeatherData()
        //            {
        //                City = cityName.ToLower(),
        //                //获取当前行日期
        //                Date = date,
        //                WeatherType = weatherType,
        //                DayTemperature = dayTemperature,
        //                NightTemperature = nightTemperature,
        //                WindType = windtype,
        //                WindSpeed = windspeed
        //            };
        //            //打印Log
        //            var contentLog =
        //                $"{cityName}=>{weather.Date}:{weather.WeatherType},{weather.DayTemperature},{weather.NightTemperature},{weather.WindType},{weather.WindSpeed}";
        //            //打印Log
        //            Console.WriteLine(contentLog);
        //            _httpDataMethodService.SetLog(contentLog, WeatherStatic.DataSetLog);
        //            InsertOrUpdate(weather);
        //        }
        //    }
        //}

        public void SetWeather(int cityId, int year, int month)
        {
            var cityName = ((WeatherStatic.CityType)cityId).ToString().ToLower();
            var model = _httpDataMethodService.GetWeather(cityId, year, month);
            if (model != null)
            {
                if (model.tqInfo.Count - 1 != DateTime.DaysInMonth(year, month) && month != DateTime.Now.Month)
                    _httpDataMethodService.SetLog($"tianqi.2345.com->{cityName}-{year}年{month}月的数据有缺失", WeatherStatic.DataLostLog);
                foreach (var weatherInfo in model.tqInfo)
                {
                    if (weatherInfo.ymd == null)
                        continue;
                    var entity = new WeatherData()
                    {
                        City = cityName,
                        //获取当前行日期
                        Date = DateTime.Parse(weatherInfo.ymd),
                        WeatherType = weatherInfo.tianqi,
                        DayTemperature = weatherInfo.bWendu,
                        NightTemperature = weatherInfo.yWendu,
                        WindType = weatherInfo.fengxiang,
                        WindSpeed = weatherInfo.fengli
                    };
                    //Log
                    var contentLog =
                        $"{cityName}=>{entity.Date}:{entity.WeatherType},{entity.DayTemperature},{entity.NightTemperature},{entity.WindType},{entity.WindSpeed}";
                    ////打印Log
                    //Console.WriteLine(contentLog);
                    //_httpDataMethodService.SetLog(contentLog, WeatherStatic.DataSetLog);
                    Insert(entity);
                }
            }
        }

        public void SetWeatherByApi(int cityId, string cityName)
        {
            try
            {
                var model = _httpDataMethodService.GetWeatherByApi(cityId, cityName);
                var data = model.data.FirstOrDefault(x => x.date == DateTime.Now.Date);
                if (data != null)
                {
                    var entity = new WeatherData()
                    {
                        City = cityName.ToLower(),
                        //获取当前行日期
                        Date = data.date,
                        WeatherType = data.wea,
                        DayTemperature = data.tem1,
                        NightTemperature = data.tem2,
                        WindType = data.wind,
                        WindSpeed = data.winspeed
                    };
                    Insert(entity);
                    //var contentLog =
                    //  $"{cityName}=>{entity.Date}:{entity.WeatherType},{entity.DayTemperature},{entity.NightTemperature},{entity.WindType},{entity.WindSpeed}";
                    //_httpDataMethodService.SetLog(contentLog, WeatherStatic.DataSetLog);
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Insert(WeatherData entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                var query = from w in _dataContext.WeatherSet
                            where w.Date == entity.Date && w.City == entity.City
                            select w;
                var weather = query.FirstOrDefault();
                if (weather == null)
                {
                    _dataContext.WeatherSet.Add(entity);
                    _dataContext.SaveChanges();
                }
            }
            catch (DbUpdateException exception)
            {
                //ensure that the detailed error text is saved in the Log
                throw new Exception(exception.Message);
            }
        }
    }
}
