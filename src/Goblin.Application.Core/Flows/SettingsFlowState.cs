using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Application.Core.Flows;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SettingsFlowState
{
    [EnumMember(Value = "narfu_group")]
    NarfuGroup,

    [EnumMember(Value = "weather_city")]
    WeatherCity,

    [EnumMember(Value = "mailing")]
    Mailing,

    [EnumMember(Value = "mailing_schedule")]
    MailingSchedule,

    [EnumMember(Value = "mailing_weather")]
    MailingWeather
}
