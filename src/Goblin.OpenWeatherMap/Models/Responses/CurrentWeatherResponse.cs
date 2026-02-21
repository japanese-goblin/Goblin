using System.Text;
using System.Text.Json.Serialization;
using Goblin.OpenWeatherMap.Models.Current;

namespace Goblin.OpenWeatherMap.Models.Responses;

public class CurrentWeatherResponse
{
    /// <summary>
    /// Координаты
    /// </summary>
    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    /// <summary>
    /// Внутреннее описание погоды
    /// </summary>
    [JsonPropertyName("weather")]
    public Weather[] Info { get; set; }

    /// <summary>
    /// Внутренний параметр (возможно, откуда берутся данные)
    /// </summary>
    [JsonPropertyName("base")]
    public string Base { get; set; }

    /// <summary>
    /// Данные о текущей погоде
    /// </summary>
    [JsonPropertyName("main")]
    public CurrentWeatherData Weather { get; set; }

    /// <summary>
    /// Дальность видимости (в метрах)
    /// </summary>
    [JsonPropertyName("visibility")]
    public double Visibility { get; set; }

    /// <summary>
    /// Информация о ветре
    /// </summary>
    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    /// <summary>
    /// Информация об облаках
    /// </summary>
    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; }

    /// <summary>
    /// Информация о дожде
    /// </summary>
    [JsonPropertyName("rain")]
    public Precipitation Rain { get; set; }

    /// <summary>
    /// Информация о снеге
    /// </summary>
    [JsonPropertyName("snow")]
    public Precipitation Snow { get; set; }

    /// <summary>
    /// Время получения погоды со станции
    /// </summary>
    [JsonPropertyName("dt"), JsonConverter(typeof(UnixTimeConverter))]
    public DateTimeOffset UnixTime { get; set; }

    /// <summary>
    /// Системная информация
    /// </summary>
    [JsonPropertyName("sys")]
    public Sys OtherInfo { get; set; }

    /// <summary>
    /// Идентификатор горожа
    /// </summary>
    [JsonPropertyName("id")]
    public int CityId { get; set; }

    /// <summary>
    /// Название города
    /// </summary>
    [JsonPropertyName("name")]
    public string CityName { get; set; }

    /// <summary>
    /// Код ответа
    /// </summary>
    [JsonPropertyName("cod")]
    public int ResponseCode { get; set; }

    /// <summary>
    /// Разница между местным временем и UTC в секундах
    /// </summary>
    [JsonPropertyName("timezone")]
    public int TimezoneDifference { get; set; }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();

        strBuilder.Append($"Погода в городе {CityName} на данный момент:").AppendLine()
                  .Append($"Описание: {Info[0].Description}").AppendLine()
                  .Append($"Температура: {Weather.Temperature:+#;-#;0}°С");
        if(Weather.FeelsLike.HasValue)
        {
            strBuilder.Append($" (ощущается как {Weather.FeelsLike:+#;-#;0}°С)");
        }

        strBuilder.AppendLine()
                  .Append($"Влажность: {Weather.Humidity}%").AppendLine()
                  .Append($"Ветер: {Wind.Speed:N0} м/с").AppendLine()
                  .Append($"Давление: {Weather.Pressure * Defaults.PressureConvert:N0} мм.рт.ст")
                  .AppendLine()
                  .Append($"Облачность: {Clouds.Cloudiness}%").AppendLine()
                  .Append($"Видимость: {Visibility} метров").AppendLine()
                  .AppendLine()
                  .Append($"Восход: {OtherInfo.Sunrise.AddSeconds(TimezoneDifference):HH:mm}").AppendLine()
                  .Append($"Закат: {OtherInfo.Sunset.AddSeconds(TimezoneDifference):HH:mm}").AppendLine()
                  .AppendLine()
                  .Append($"Данные обновлены {UnixTime.AddSeconds(TimezoneDifference):dd.MM.yyyy HH:mm}");

        return strBuilder.ToString();
    }
}