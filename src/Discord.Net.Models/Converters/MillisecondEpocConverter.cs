using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class MillisecondEpocConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return default;
            case JsonTokenType.Number:
                return DateTimeOffset.UnixEpoch.AddMilliseconds(reader.GetUInt64());
            case JsonTokenType.String:
                var str = reader.GetString();

                if (str is null) return default;
                
                if (long.TryParse(str, out var milliseconds))
                    return DateTimeOffset.UnixEpoch.AddMilliseconds(milliseconds);
                
                return DateTimeOffset.Parse(str);
            default:
                throw new JsonException($"Expected Null, Number, or String for DateTimeOffset; got {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((ulong)Math.Floor((value - DateTimeOffset.UnixEpoch).TotalMilliseconds));
    }
}
