using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Current;

public class CurrentWeatherData
{
    /// <summary>
    /// Атмосферное давление на уровне земли, гПа (гектопаскали)
    /// </summary>
    [JsonPropertyName("grnd_level")]
    public double? GroundLevel { get; set; }

    /// <summary>
    /// Атмосферное давление на уровне моря, гПа (гектопаскали)
    /// </summary>
    [JsonPropertyName("sea_level")]
    public double? SeaLevel { get; set; }

    /// <summary>
    /// Температура
    /// </summary>
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    /// <summary>
    /// Атмосферное давление (на уровне моря, если нет данных о <see cref="GroundLevel"/> или <see cref="SeaLevel"/>)
    /// </summary>
    [JsonPropertyName("pressure")]
    public double Pressure { get; set; }

    /// <summary>
    /// Влажность
    /// </summary>
    [JsonPropertyName("humidity")]
    public double Humidity { get; set; }

    /// <summary>
    /// Минимальная температура
    /// Это минимальная наблюдаемая в настоящее время температура (в пределах крупных мегаполисов и городских районов).
    /// </summary>
    [JsonPropertyName("temp_min")]
    public double MinTemp { get; set; }

    /// <summary>
    /// Максимальная температура
    /// Это минимальная наблюдаемая в настоящее время температура (в пределах крупных мегаполисов и городских районов).
    /// </summary>
    [JsonPropertyName("temp_max")]
    public double MaxTemp { get; set; }

    /// <summary>
    /// Температура, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonPropertyName("feels_like")]
    public double? FeelsLike { get; set; }
}