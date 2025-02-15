﻿using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class DailyWeatherListItem
{
    /// <summary>
    /// Дата погоды
    /// </summary>
    [JsonPropertyName("dt")]
    [JsonConverter(typeof(UnixTimeConverter))]
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

        strBuilder.AppendFormat("Температура: от {0:+#;-#;0}°С до {1:+#;-#;0}°С", Temperature.Min,
                                Temperature.Max).AppendLine();
        strBuilder.AppendFormat("Утром: {0:+#;-#;0}°С", Temperature.Morning).AppendLine();
        strBuilder.AppendFormat("Днём: {0:+#;-#;0}°С", Temperature.Day).AppendLine();
        strBuilder.AppendFormat("Вечером: {0:+#;-#;0}°С", Temperature.Evening).AppendLine();
        strBuilder.AppendFormat("Ночью: {0:+#;-#;0}°С", Temperature.Night).AppendLine();
        strBuilder.AppendLine();
        strBuilder.AppendFormat("Описание: {0}", Weather[0].Description).AppendLine();
        strBuilder.AppendFormat("Влажность: {0}%", Humidity).AppendLine();
        strBuilder.AppendFormat("Ветер: {0:N0} м/с", WindSpeed).AppendLine();
        strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", Pressure * Defaults.PressureConvert).AppendLine();
        strBuilder.AppendFormat("Облачность: {0}%", Cloudiness).AppendLine();

        return strBuilder.ToString();
    }
}