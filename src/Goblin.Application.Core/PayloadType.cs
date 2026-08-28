using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Application.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PayloadType
{
    [EnumMember(Value = "init")]
    Init,

    [EnumMember(Value = "menu")]
    Menu,

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