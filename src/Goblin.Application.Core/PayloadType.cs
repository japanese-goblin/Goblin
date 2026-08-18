using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Application.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PayloadType
{
    [EnumMember(Value = "init")]
    Init
}