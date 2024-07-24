using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class SnowflakeConverter : JsonConverter<ulong>
{
    public static readonly SnowflakeConverter Instance = new();

    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType is JsonTokenType.String
            ? ulong.Parse(reader.GetString() ?? throw new InvalidOperationException("Expected non-empty string for snowflake"))
            : reader.GetUInt64();
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        // write as string, always
        writer.WriteStringValue(value.ToString());
    }
}
