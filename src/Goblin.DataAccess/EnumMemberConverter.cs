using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Goblin.DataAccess;

public class EnumMemberConverter<TEnum>() : ValueConverter<TEnum, string>(
    value => GetEnumMemberValue(value),
    value => ParseEnumMemberValue(value)
) where TEnum : struct, Enum
{
    private static string GetEnumMemberValue(TEnum value)
    {
        var member = typeof(TEnum).GetMember(value.ToString()).Single();

        var attribute = member.GetCustomAttribute<EnumMemberAttribute>();

        return attribute?.Value ?? value.ToString();
    }

    private static TEnum ParseEnumMemberValue(string value)
    {
        foreach (var member in typeof(TEnum).GetMembers(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = member.GetCustomAttribute<EnumMemberAttribute>();

            if (attribute?.Value == value)
                return Enum.Parse<TEnum>(member.Name);
        }

        return Enum.Parse<TEnum>(value);
    }
}
