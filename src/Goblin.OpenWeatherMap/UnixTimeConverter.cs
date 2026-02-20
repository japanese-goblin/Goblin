using System.Text.Json;
using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap;

internal sealed class UnixTimeConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var seconds = reader.GetInt64();
        return DateTimeOffset.UnixEpoch.AddSeconds(seconds);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}