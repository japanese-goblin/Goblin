using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlowType
{
    [EnumMember(Value = "start")]
    Start,

    [EnumMember(Value = "main-menu")]
    MainMenu
}