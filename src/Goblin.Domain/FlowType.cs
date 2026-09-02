using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlowType
{
    [EnumMember(Value = "start")]
    Start,

    [EnumMember(Value = "main-menu")]
    MainMenu,

    [EnumMember(Value = "schedule")]
    Schedule,

    [EnumMember(Value = "exams")]
    Exams,

    [EnumMember(Value = "current_weather")]
    CurrentWeather,

    [EnumMember(Value = "forecast_weather")]
    ForecastWeather,

    [EnumMember(Value = "settings")]
    Settings
}
