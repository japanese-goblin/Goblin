using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Goblin.Domain;

/// <summary>
///     Тип потребителя
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConsumerType
{
    /// <summary>
    ///     ВК
    /// </summary>
    [EnumMember(Value = "vk")]
    Vkontakte,

    /// <summary>
    ///     Telegram
    /// </summary>
    [EnumMember(Value = "telegram")]
    Telegram
}