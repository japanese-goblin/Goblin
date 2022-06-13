using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models;

public class Weather
{
    /// <summary>
    /// Описание погоды
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Иконка погоды
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// Идентификатор состояния погоды
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// Группа погодных условий (?)
    /// </summary>
    [JsonPropertyName("main")]
    public string Main { get; set; }
}