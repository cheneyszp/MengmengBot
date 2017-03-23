using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MengmengBot.Models
{
    public class WeatherData
    {
        [JsonProperty(PropertyName = "HeWeather data service 3.0")]
        public HeweatherDataService30[] HeWeatherdataservice30 { get; set; }
    }

    public class HeweatherDataService30
    {
        public Aqi aqi { get; set; }
        public Basic basic { get; set; }
        public Daily_Forecast[] daily_forecast { get; set; }
        public Hourly_Forecast[] hourly_forecast { get; set; }
        public Now now { get; set; }
        public string status { get; set; }
        public Suggestion suggestion { get; set; }
    }

    public class Aqi
    {
        public City city { get; set; }
    }

    public class City
    {
        public string aqi { get; set; }
        public string co { get; set; }
        public string no2 { get; set; }
        public string o3 { get; set; }
        public string pm10 { get; set; }
        public string pm25 { get; set; }
        public string qlty { get; set; }
        public string so2 { get; set; }
    }

    public class Basic
    {
        public string city { get; set; }
        public string cnty { get; set; }
        public string id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public Update update { get; set; }
    }

    public class Update
    {
        public string loc { get; set; }
        public string utc { get; set; }
    }

    public class Now
    {
        public Cond cond { get; set; }
        public string fl { get; set; }
        public string hum { get; set; }
        public string pcpn { get; set; }
        public string pres { get; set; }
        public string tmp { get; set; }
        public string vis { get; set; }
        public Wind wind { get; set; }
    }

    public class Cond
    {
        public string code { get; set; }
        public string txt { get; set; }
    }

    public class Wind
    {
        public string deg { get; set; }
        public string dir { get; set; }
        public string sc { get; set; }
        public string spd { get; set; }
    }

    public class Suggestion
    {
        public Comf comf { get; set; }
        public Cw cw { get; set; }
        public Drsg drsg { get; set; }
        public Flu flu { get; set; }
        public Sport sport { get; set; }
        public Trav trav { get; set; }
        public Uv uv { get; set; }
    }

    public class Comf
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Cw
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Drsg
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Flu
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Sport
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Trav
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Uv
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

    public class Daily_Forecast
    {
        public Astro astro { get; set; }
        public Cond1 cond { get; set; }
        public string date { get; set; }
        public string hum { get; set; }
        public string pcpn { get; set; }
        public string pop { get; set; }
        public string pres { get; set; }
        public Tmp tmp { get; set; }
        public string vis { get; set; }
        public Wind1 wind { get; set; }
    }

    public class Astro
    {
        public string sr { get; set; }
        public string ss { get; set; }
    }

    public class Cond1
    {
        public string code_d { get; set; }
        public string code_n { get; set; }
        public string txt_d { get; set; }
        public string txt_n { get; set; }
    }

    public class Tmp
    {
        public string max { get; set; }
        public string min { get; set; }
    }

    public class Wind1
    {
        public string deg { get; set; }
        public string dir { get; set; }
        public string sc { get; set; }
        public string spd { get; set; }
    }

    public class Hourly_Forecast
    {
        public string date { get; set; }
        public string hum { get; set; }
        public string pop { get; set; }
        public string pres { get; set; }
        public string tmp { get; set; }
        public Wind2 wind { get; set; }
    }

    public class Wind2
    {
        public string deg { get; set; }
        public string dir { get; set; }
        public string sc { get; set; }
        public string spd { get; set; }
    }

}