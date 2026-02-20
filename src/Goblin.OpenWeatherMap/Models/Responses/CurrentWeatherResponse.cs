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

        strBuilder.AppendFormat("Погода в городе {0} на данный момент:", CityName).AppendLine()
                  .AppendFormat("Описание: {0}", Info[0].Description).AppendLine()
                  .AppendFormat("Температура: {0:+#;-#;0}°С", Weather.Temperature);
        if(Weather.FeelsLike.HasValue)
        {
            strBuilder.AppendFormat(" (ощущается как {0:+#;-#;0}°С)", Weather.FeelsLike);
        }

        strBuilder.AppendLine()
                  .AppendFormat("Влажность: {0}%", Weather.Humidity).AppendLine()
                  .AppendFormat("Ветер: {0:N0} м/с", Wind.Speed).AppendLine()
                  .AppendFormat("Давление: {0:N0} мм.рт.ст", Weather.Pressure * Defaults.PressureConvert)
                  .AppendLine()
                  .AppendFormat("Облачность: {0}%", Clouds.Cloudiness).AppendLine()
                  .AppendFormat("Видимость: {0} метров", Visibility).AppendLine()
                  .AppendLine()
                  .AppendFormat("Восход: {0:HH:mm}", OtherInfo.Sunrise.AddSeconds(TimezoneDifference)).AppendLine()
                  .AppendFormat("Закат: {0:HH:mm}", OtherInfo.Sunset.AddSeconds(TimezoneDifference)).AppendLine()
                  .AppendLine()
                  .AppendFormat("Данные обновлены {0:dd.MM.yyyy HH:mm}", UnixTime.AddSeconds(TimezoneDifference));

        return strBuilder.ToString();
    }
}