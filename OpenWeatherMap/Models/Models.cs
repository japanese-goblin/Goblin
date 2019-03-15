using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenWeatherMap.Models
{
    internal class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    internal class Weather
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("main")]
        private string Main { get; set; }

        [JsonProperty("description")]
        public string State { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    internal class Main
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("temp_min")]
        public double MinTemp { get; set; }

        [JsonProperty("temp_max")]
        public double MaxTemp { get; set; }
    }

    internal class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public double Degrees { get; set; }
    }

    internal class Clouds
    {
        [JsonProperty("all")]
        public int Cloudiness { get; set; }
    }

    internal class Sys
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public double Message { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("sunrise")]
        public int Sunrise { get; set; }

        [JsonProperty("sunset")]
        public int Sunset { get; set; }
    }

    internal class WeatherInfo
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
        public double Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("dt")]
        public int UnixTime { get; set; }

        [JsonProperty("sys")]
        public Sys OtherInfo { get; set; }

        [JsonProperty("id")]
        private int Id { get; set; }

        [JsonProperty("name")]
        public string CityName { get; set; }

        [JsonProperty("cod")]
        public int CityCode { get; set; }
    }
}