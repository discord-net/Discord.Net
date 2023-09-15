using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public class ColorConverter : JsonConverter<Color>
{
    public static readonly ColorConverter Instance = new();


    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return default;

        return uint.TryParse(reader.GetString(), out var result)
            ? new Color(result)
            : default;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"#{(uint)value:X}");
    }
}
