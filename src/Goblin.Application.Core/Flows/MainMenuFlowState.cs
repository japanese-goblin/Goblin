using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Application.Core.Flows;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MainMenuFlowState
{
    [EnumMember(Value = "schedule")]
    Schedule,
    
    [EnumMember(Value = "exams")]
    Exams,
    
    [EnumMember(Value = "current_weather")]
    CurrentWeather,
    
    [EnumMember(Value = "forecast_weather")]
    ForecastWeather,
    
    [EnumMember(Value = "settings")]
    Settings,
}