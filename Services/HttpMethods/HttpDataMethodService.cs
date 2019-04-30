using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Weather.Infrastructure.Cache;
using Weather.Models;

namespace Weather.Services.HttpMethods
{
    public class HttpDataMethodService : IHttpDataMethodService
    {
        public static HttpClient HttpClient = new HttpClient(new HttpClientHandler()
        { AutomaticDecompression = DecompressionMethods.GZip });

        private static readonly Func<string> _getFileName = () => DateTime.Now.ToString("yyyyMM");

        #region HttpMethod

        public async Task<string> GetHtml(Uri uri, string proxy = null)
        {
            return await Task.Run(() =>
            {
                var pageSource = string.Empty;
                var watch = new Stopwatch();
                watch.Start();
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Accept = "*/*";
                request.ServicePoint.Expect100Continue = false; //加快载入速度
                request.ServicePoint.UseNagleAlgorithm = false; //禁止Nagle算法加快载入速度
                request.AllowWriteStreamBuffering = false; //禁止缓冲加快载入速度
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate"); //定义gzip压缩页面支持
                request.ContentType = "application/x-www-form-urlencoded"; //定义文档类型及编码
                request.AllowAutoRedirect = false; //禁止自动跳转
                //设置User-Agent，伪装成Google Chrome浏览器
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
                request.Timeout = 1000 * 10; //定义请求超时时间为10秒
                request.KeepAlive = true; //启用长连接
                request.Method = "GET"; //定义请求方式为GET              
                if (proxy != null) request.Proxy = new WebProxy(proxy); //设置代理服务器IP，伪装请求地址
                request.ServicePoint.ConnectionLimit = int.MaxValue; //定义最大连接数

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.ToLower().Contains("gzip")) //解压
                    {
                        using (
                            GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
                        )
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding != null && response.ContentEncoding.ToLower().Contains("deflate")) //解压
                    {
                        using (
                            DeflateStream stream = new DeflateStream(response.GetResponseStream(),
                                CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }

                        }
                    }
                    else
                    {
                        //获取请求响应
                        using (var stream = response.GetResponseStream()) //原始
                        {
                            if (stream != null)
                                using (
                                    var reader = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                                {
                                    pageSource = reader.ReadToEnd();
                                }
                        }
                    }
                }
                request.Abort();
                watch.Stop();
                //获取请求执行时间
                var milliseconds = watch.ElapsedMilliseconds;
                //Console.WriteLine($"请求执行时间:{milliseconds}");

                return pageSource;
            });
        }

        public HtmlDocument GetHtmlDoc(Uri uri)
        {
            try
            {
                var docHtml = GetHtml(uri).Result;
                var doc = new HtmlDocument();
                doc.LoadHtml(docHtml);
                SetLog($"数据抓取成功，抓取地址：{uri.AbsoluteUri}");
                Thread.Sleep(1000);
                return doc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接异常：{ex.Message}，正在重试");
                SetLog($"数据抓取失败，抓取地址：{uri.AbsolutePath}，失败原因：{ex.Message}");
                Thread.Sleep(2000);
                return GetHtmlDoc(uri);
            }
        }

        public T Get<T>(string url)
        {
            var res = HttpClient.GetAsync(url).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(result);
            }
            else
                throw new Exception($"请求失败，失败原因：{res.ReasonPhrase}");
        }

        public T Post<T>(string data, string url)
        {
            var res = HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            }).Result;
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(result);
            }
            else
                throw new Exception($"请求失败，失败原因：{res.ReasonPhrase}");

        }

        #endregion

        #region TianQiApi

        public WeatherApiModel GetWeatherByApi(int cityId, string cityName)
        {
            var version = "v1";
            var url = $"{WeatherStatic.TianQiApi}?version={version}&cityid={cityId}&city={cityName}";

            return Get<WeatherApiModel>(url);
        }

        public TianQiApi GetWeather(int cityId, int year, int month)
        {
            var date = new DateTime(year, month, 1).ToString("yyyyMM");
            var url1 = string.Format(WeatherStatic.ApiWeatherUrl2, date, cityId, date);
            var url2 = string.Format(WeatherStatic.ApiWeatherUrl1, cityId, $"{year}{month}");
            var res1 = HttpClient.GetAsync(url1).Result;
            if (res1.IsSuccessStatusCode)
            {
                return DataHandle(res1);
            }
            else if (res1.StatusCode == HttpStatusCode.NotFound)
            {
                //尝试其他地址
                var res2 = HttpClient.GetAsync(url2).Result;
                if (res2.IsSuccessStatusCode)
                {
                    return DataHandle(res2);
                }
                else if (res1.StatusCode == HttpStatusCode.NotFound)
                {
                    SetLog($"数据获取失败，抓取地址：{url1}And{url2}，失败原因：{res2.ReasonPhrase}");
                    return null;
                }
                else
                {
                    throw new Exception($"数据获取失败，抓取地址：{url1}And{url2}，失败原因：{res2.ReasonPhrase}");
                }
            }
            else
            {
                throw new Exception($"数据获取失败，抓取地址：{url1}And{url2}，失败原因：{res1.ReasonPhrase}");
            }
        }

        public TianQiApi DataHandle(HttpResponseMessage res)
        {
            var bytes = res.Content.ReadAsByteArrayAsync().Result;
            var result = Encoding.GetEncoding("GB2312").GetString(bytes);

            if (!string.IsNullOrWhiteSpace(result))
            {
                //json不规范
                result = result.Substring(result.IndexOf('{'));
                result = result.Substring(0, result.Length - 1);
                //Key添加引号
                var reg = new Regex(@"(\w+)(\s*:\s*)");
                result = reg.Replace(result, @"'$1'$2").Replace(@"'", @"""");
            }
            Thread.Sleep(500);
            return JsonConvert.DeserializeObject<TianQiApi>(result);
        }


        #endregion

        #region SetLog

        public void SetLog(string content, string fileName = null)
        {
            try
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                //var directory = Directory.GetCurrentDirectory();
                var logDirPath = $"{directory}\\Log";
                if (!Directory.Exists(logDirPath))
                {
                    Directory.CreateDirectory(logDirPath);
                }
                if (string.IsNullOrEmpty(fileName))
                    fileName = _getFileName();
                var logPath = $"{logDirPath}\\Log_{fileName} .txt";

                StreamWriter sr;
                if (File.Exists(logPath)) //如果文件存在,则创建File.AppendText对象
                {
                    sr = File.AppendText(logPath);
                }
                else //如果文件不存在,则创建File.CreateText对象
                {
                    sr = File.CreateText(logPath);
                }

                sr.WriteLine(content);
                sr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }


        }

        #endregion


    }
}
