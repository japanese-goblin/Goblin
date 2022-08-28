using System;
using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Sys
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("message")]
    public double Message { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("sunrise")]
    [JsonConverter(typeof(UnixTimeConverter))]
    public DateTimeOffset Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    [JsonConverter(typeof(UnixTimeConverter))]
    public DateTimeOffset Sunset { get; set; }
}