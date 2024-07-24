using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class MillisecondEpocConverter : JsonConverter<DateTimeOffset>
{
    public static readonly MillisecondEpocConverter Instance = new();

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType is JsonTokenType.Null
            ? DateTimeOffset.MinValue
            : DateTimeOffset.UnixEpoch.AddMilliseconds(reader.GetUInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((ulong)Math.Floor((value - DateTimeOffset.UnixEpoch).TotalMilliseconds));
    }
}
