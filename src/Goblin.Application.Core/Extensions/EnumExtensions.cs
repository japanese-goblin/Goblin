using System.Reflection;
using System.Runtime.Serialization;

namespace Goblin.Application.Core.Extensions;

internal static class EnumExtensions
{
    public static string GetEnumMemberValue<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        var member = typeof(TEnum)
            .GetMember(value.ToString())
            .FirstOrDefault();

        return member?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? value.ToString();
    }
}
