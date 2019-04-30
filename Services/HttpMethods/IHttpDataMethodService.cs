using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Weather.Models;

namespace Weather.Services.HttpMethods
{
    public interface IHttpDataMethodService
    {
        Task<string> GetHtml(Uri uri, string proxy = null);

        HtmlDocument GetHtmlDoc(Uri uri);

        T Get<T>(string url);

        T Post<T>(string data, string url);

        void SetLog(string content, string fileName = null);

        WeatherApiModel GetWeatherByApi(int cityId, string cityName);

        TianQiApi GetWeather(int cityId, int year, int month);
    }
}
