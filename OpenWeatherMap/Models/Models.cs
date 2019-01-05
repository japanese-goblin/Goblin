using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenWeatherMap.Models
{
    internal class Coord
    {
        [JsonProperty("lon")] public double Longitude { get; set; }
        [JsonProperty("lat")] public double Latitude { get; set; }
    }

    internal class Weather
    {
        [JsonProperty("id")] public int ID { get; set; }
        [JsonProperty("main")] private string _main { get; set; }
        [JsonProperty("description")] public string State { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; }
    }

    internal class Main
    {
        [JsonProperty("temp")] private double _temperature { get; set; }
        [JsonProperty("pressure")] private double _pressure { get; set; }
        [JsonProperty("humidity")] private double _humidity { get; set; }
        [JsonProperty("temp_min")] private double _minTemp { get; set; }
        [JsonProperty("temp_max")] private double _maxTemp { get; set; }

        public string Temperature => $"{Math.Round(_temperature)}°С";
        public string Pressure => $"{(int)(_pressure * 0.75006375541921)} мм.рт.ст.";
        public string Humidity => $"{_humidity}%";
        public string MinTemp => $"{_minTemp}°С";
        public string MaxTemp => $"{_maxTemp}°С";
    }

    internal class Wind
    {
        [JsonProperty("speed")] public double Speed { get; set; }
        [JsonProperty("deg")] public double Degrees { get; set; }

        [JsonIgnore] public string SpeedInfo => $"{Math.Round(Speed)} м/с";
    }

    internal class Clouds
    {
        [JsonProperty("all")] private int _all { get; set; }

        public string Cloudiness => $"{_all}%";
    }

    internal class Sys
    {
        [JsonProperty("type")] public int Type { get; set; }
        [JsonProperty("id")] public int ID { get; set; }
        [JsonProperty("message")] public double Message { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("sunrise")] private int _sunrise { get; set; }
        [JsonProperty("sunset")] private int _sunset { get; set; }

        public DateTime Sunrise => OpenWeatherMap.WeatherInfo.UnixToDateTime(_sunrise);
        public DateTime Sunset => OpenWeatherMap.WeatherInfo.UnixToDateTime(_sunset);
    }

    internal class WeatherInfo
    {
        [JsonProperty("coord")] private Coord Coord { get; set; }
        [JsonProperty("weather")] public List<Weather> Info { get; set; }
        [JsonProperty("base")] public string BaseInfo { get; set; }
        [JsonProperty("main")] public Main Weather { get; set; }
        [JsonProperty("visibility")] private double _visibility { get; set; }
        [JsonProperty("wind")] public Wind Wind { get; set; }
        [JsonProperty("clouds")] public Clouds Clouds { get; set; }
        [JsonProperty("dt")] public int UnixTime { get; set; }
        [JsonProperty("sys")] public Sys OtherInfo { get; set; }
        [JsonProperty("id")] private int ID { get; set; }
        [JsonProperty("name")] public string CityName { get; set; }
        [JsonProperty("cod")] public int CityCode { get; set; }

        public string Visibility => $"{_visibility} метров";
    }
}
