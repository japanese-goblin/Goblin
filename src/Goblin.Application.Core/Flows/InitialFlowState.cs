using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Application.Core.Flows;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InitialFlowState
{
    [EnumMember(Value = "narfu_group")]
    SettingNarfuGroup,

    [EnumMember(Value = "city_weather")]
    SettingWeather,

    [EnumMember(Value = "continue")]
    Continue,

    [EnumMember(Value = "skip")]
    Skip
}