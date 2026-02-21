using System.Text;
using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class DailyWeatherListItem
{
    /// <summary>
    /// Дата погоды
    /// </summary>
    [JsonPropertyName("dt"), JsonConverter(typeof(UnixTimeConverter))]
    public DateTimeOffset UnixTime { get; set; }

    /// <summary>
    /// Температура
    /// </summary>
    [JsonPropertyName("temp")]
    public Temperature Temperature { get; set; }

    /// <summary>
    /// Температура с учётом человеческих ощущений
    /// </summary>
    [JsonPropertyName("feels_like")]
    public FeelsLike FeelsLike { get; set; }

    /// <summary>
    /// Атмосферное давление
    /// </summary>
    [JsonPropertyName("pressure")]
    public double Pressure { get; set; }

    /// <summary>
    /// Влажность
    /// </summary>
    [JsonPropertyName("humidity")]
    public long Humidity { get; set; }

    /// <summary>
    /// Внутреннее описание погоды
    /// </summary>
    [JsonPropertyName("weather")]
    public Weather[] Weather { get; set; }

    /// <summary>
    /// Скорость ветра
    /// </summary>
    [JsonPropertyName("speed")]
    public double WindSpeed { get; set; }

    /// <summary>
    /// Направление ветра, в градусах
    /// </summary>
    [JsonPropertyName("deg")]
    public long WindDeg { get; set; }

    /// <summary>
    /// Облачность, в %
    /// </summary>
    [JsonPropertyName("clouds")]
    public long Cloudiness { get; set; }

    /// <summary>
    /// Количество дождя, в миллиметрах
    /// </summary>
    [JsonPropertyName("rain")]
    public double? Rain { get; set; }

    /// <summary>
    /// Количество снега, в миллиметрах
    /// </summary>
    [JsonPropertyName("snow")]
    public double? Snow { get; set; }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();

        strBuilder.Append($"Температура: от {Temperature.Min:+#;-#;0}°С до {Temperature.Max:+#;-#;0}°С").AppendLine();
        strBuilder.Append($"Утром: {Temperature.Morning:+#;-#;0}°С").AppendLine();
        strBuilder.Append($"Днём: {Temperature.Day:+#;-#;0}°С").AppendLine();
        strBuilder.Append($"Вечером: {Temperature.Evening:+#;-#;0}°С").AppendLine();
        strBuilder.Append($"Ночью: {Temperature.Night:+#;-#;0}°С").AppendLine();
        strBuilder.AppendLine();
        strBuilder.Append($"Описание: {Weather[0].Description}").AppendLine();
        strBuilder.Append($"Влажность: {Humidity}%").AppendLine();
        strBuilder.Append($"Ветер: {WindSpeed:N0} м/с").AppendLine();
        strBuilder.Append($"Давление: {Pressure * Defaults.PressureConvert:N0} мм.рт.ст").AppendLine();
        strBuilder.Append($"Облачность: {Cloudiness}%").AppendLine();

        return strBuilder.ToString();
    }
}