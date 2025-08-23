using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Models.Json.Converters;

public sealed class ImageDataConverter : JsonConverter<ImageData>
{
    public static readonly ImageDataConverter Instance = new();
    
    public override ImageData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, ImageData value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}