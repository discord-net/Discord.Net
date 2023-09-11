using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Rest.Converters;

public class UInt64Converter : JsonConverter<ulong>
{
    public static readonly UInt64Converter Instance = new();

    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetUInt64();
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
