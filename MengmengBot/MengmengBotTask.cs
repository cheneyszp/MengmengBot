using MengmengBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MengmengBot
{
    public class MengmengBotTask
    {
        public static async Task<String> GetStockRateAsync(string StockSymbol)
        {
            try
            {
                string pattern = @"^[a-zA-Z]*$";
                if (StockSymbol.Length == 4 && System.Text.RegularExpressions.Regex.IsMatch(StockSymbol, pattern))//判断字符串长度  
                {
                    String result = $"请查看："+"http://gu.sina.cn/us/hq/quotes.php?code=" + StockSymbol + "&from=pc";
                    return result;
                }
                return null;
                
                //string ServiceURL = $"http://finance.yahoo.com/d/quotes.csv?s={StockSymbol}&f=sl1d1nd";
                //string ResultInCSV;
                //using (WebClient client = new WebClient())
                //{
                //    ResultInCSV = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
                //}
                //var FirstLine = ResultInCSV.Split('\n')[0];
                //var Price = FirstLine.Split(',')[1];
                //if (Price != null && Price.Length >= 0)
                //{
                //    double result;
                //    if (double.TryParse(Price, out result))
                //    {
                //        return result;
                //    }
                //}
                //return null;
            }
            catch (WebException ex)
            {
                //handle your exception here  
                throw ex;
            }
        }

        public static async Task<WeatherData> GetWeatherAsync(string city)
        {
            try
            {
                string ServiceURL = $"https://free-api.heweather.com/v5/weather?city={city}&lang=zh&key=YOUR_WEATHER_KEY";
                string ResultString;
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    ResultString = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
                }
                WeatherData weatherData = (WeatherData)JsonConvert.DeserializeObject(ResultString, typeof(WeatherData)); 
                return weatherData;
            }
            catch (WebException ex)
            {
                //handle your exception here  
                //throw ex;
                return null;
            }
        }
    }
}