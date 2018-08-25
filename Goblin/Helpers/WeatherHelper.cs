using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Goblin.Helpers
{
    public static class WeatherHelper
    {
        private const string _endPoint = "http://api.openweathermap.org/data/2.5";
        private const string _token = "***REMOVED***";

        public static string GetWeather(string city)
        {
            var result = "";
            using (var web = new WebClient())
            {
                web.Encoding = Encoding.UTF8;
                var req = $"weather?q={city}&units=metric&appid={_token}&lang=ru";
                var w = JsonConvert.DeserializeObject<WeatherInfo>(web.DownloadString($"{_endPoint}/{req}"));
                result = $"Погода в городе {city} на {UnixToDateTime(w.UnixTime).AddHours(3):dd.MMyyyy HH:mm}\n" +
                         $"Температура: {w.Weather.Temperature}\n" +
                         $"Описание погоды: {w.Info[0].State}\n" +
                         $"Влажность: {w.Weather.Humidity}\n" +
                         $"Ветер: {w.Wind.SpeedInfo}\n" +
                         $"Давление: {w.Weather.Pressure}\n" +
                         $"Видимость: {w.Visibility}";
            }

            return result;
        }

        public static bool CheckCity(string city)
        {
            var result = false;
            using (var web = new WebClient())
            {
                var req = $"weather?q={city}&units=metric&appid={_token}";
                try
                {
                    var w = JsonConvert.DeserializeObject<WeatherInfo>(web.DownloadString($"{_endPoint}/{req}"));
                    result = true;
                }
                catch { }
            }

            return result;
        }

        private static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    #region Weather classes
    class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    class Weather
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("main")]
        private string _main { get; set; }
        [JsonProperty("description")]
        public string State { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    class Main
    {
        [JsonProperty("temp")]
        private double _temperature { get; set; }
        [JsonProperty("pressure")]
        private int _pressure { get; set; }
        [JsonProperty("humidity")]
        private int _humidity { get; set; }
        [JsonProperty("temp_min")]
        private double _minTemp { get; set; }
        [JsonProperty("temp_max")]
        private double _maxTemp { get; set; }

        public string Temperature => $"{_temperature}°С";
        public string Pressure => $"{(int)(_pressure*0.75006375541921)} мм рт.ст.";
        public string Humidity => $"{_humidity}%";
        public string MinTemp => $"{_minTemp}°С";
        public string MaxTemp => $"{_maxTemp}°С";
    }

    class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
        [JsonProperty("deg")]
        public double Degrees { get; set; }

        [JsonIgnore] public string SpeedInfo => $"{Speed} метров в секунду";
    }

    class Clouds
    {
        [JsonProperty("all")]
        private int _all { get; set; }

        public string Cloudiness => $"{_all}%";
    }

    class Sys
    {
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("message")]
        public double Message { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("sunrise")]
        public int Sunrise { get; set; }
        [JsonProperty("sunset")]
        public int Sunset { get; set; }
    }

    class WeatherInfo
    {
        [JsonProperty("coord")]
        private Coord Coord { get; set; }

        [JsonProperty("weather")]
        public List<Weather> Info { get; set; }

        [JsonProperty("base")]
        public string BaseInfo { get; set; }

        [JsonProperty("main")]
        public Main Weather { get; set; }

        [JsonProperty("visibility")]
        private int _visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("dt")]
        public int UnixTime { get; set; }

        [JsonProperty("sys")]
        public Sys OtherInfo { get; set; }

        [JsonProperty("id")]
        private int ID { get; set; }

        [JsonProperty("name")]
        public string CityName { get; set; }

        [JsonProperty("cod")]
        public int CityCode { get; set; }

        public string Visibility => $"{_visibility} метров";
    }
    #endregion
}