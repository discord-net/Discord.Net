using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Rest.Converters;

public class EmbedTypeConverter : JsonConverter<EmbedType>
{
    public override EmbedType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeString = reader.GetString();

        return Enum.TryParse(typeString, true, out EmbedType value)
            ? value
            : EmbedType.Unknown;
    }

    public override void Write(Utf8JsonWriter writer, EmbedType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLower());
    }
}
