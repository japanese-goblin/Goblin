using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Sys
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
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTimeOffset Sunrise { get; set; }

    [JsonProperty("sunset")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTimeOffset Sunset { get; set; }
}