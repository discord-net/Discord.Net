using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Models.Json.Converters;

public sealed class SnowflakeConverter : JsonConverter<Snowflake>
{
    public static readonly SnowflakeConverter Instance = new SnowflakeConverter();

    public override Snowflake Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number when reader.TryGetUInt64(out var snowflake):
                return snowflake;
            case JsonTokenType.String:
                return Snowflake.Parse(reader.GetString());

            default:
                throw new JsonException($"Expected numeric or string token for snowflake, got {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}