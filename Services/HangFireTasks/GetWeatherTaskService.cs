using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Weather.Data;
using Weather.Domain;
using Weather.Infrastructure.Cache;
using Weather.Services.SetWeathers;

namespace Weather.Services.HangFireTasks
{
    public class GetWeatherTaskService : IGetWeatherTaskService
    {
        private readonly IDataParsingService _dataParsingService;
        private readonly DataContext _dataContext;

        public GetWeatherTaskService(IDataParsingService dataParsingService,
            DataContext dataContext)
        {
            _dataContext = dataContext;
            _dataParsingService = dataParsingService;
        }

        public void GetWeather()
        {
            foreach (var city in Enum.GetValues(typeof(WeatherStatic.CityType)))
            {
                var apiCityType = (WeatherStatic.ApiCityType)Enum.Parse(typeof(WeatherStatic.ApiCityType), city.ToString());
                var entity = GetLastSync(city.ToString());
                if (entity != null)
                {
                    if (entity.Date == DateTime.Now.Date)
                        continue;
                    if (entity.Date == DateTime.Now.Date.AddDays(-1))
                    {
                        _dataParsingService.SetWeatherByApi((int)apiCityType, city.ToString());
                        continue;
                    }
                    GetWeatherMethod(city, entity.Date.Year);
                    _dataParsingService.SetWeatherByApi((int)apiCityType, city.ToString());
                }
                else
                {
                    GetWeatherMethod(city);
                    _dataParsingService.SetWeatherByApi((int)apiCityType, city.ToString());
                }
            }
        }

        /// <summary>
        /// 获取该城市最后同步数据
        /// </summary>
        /// <returns></returns>
        public WeatherData GetLastSync(string cityName)
        {
            IQueryable<WeatherData> query = _dataContext.WeatherSet;
            query = query.Where(x => x.City == cityName.ToLower());
            query = query.OrderByDescending(x => x.Date);
            return query.FirstOrDefault();
        }

        public void GetWeatherMethod(object city, int year = 2011)
        {
            for (; year <= DateTime.Now.Year; year++)
            {
                for (var month = 1; month <= 12; month++)
                {
                    if (year == DateTime.Now.Year && month > DateTime.Now.Month)
                        break;
                    //Console.WriteLine($"正在抓取：{city}--{year}年{month}月");
                    //SetWeatherByHouBao(city.ToString(), year, month);
                    //_dataParsingService.SetWeatherByBaidu((int)city, year, month);
                    _dataParsingService.SetWeather((int)city, year, month);
                }
            }
        }
    }
}
