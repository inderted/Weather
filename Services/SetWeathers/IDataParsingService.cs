using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.Services.SetWeathers
{
    public interface IDataParsingService
    {
        //void SetWeatherByHouBao(string city, int year, int month);

        //void SetWeatherByBaidu(int city, int year, int month);

        void SetWeatherByApi(int cityId, string cityName);

        void SetWeather(int cityId, int year, int month);
    }
}
