using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json.Converters;

public sealed class OptionalConverter<T> : JsonConverter<Optional<T>>
{
    public static readonly OptionalConverter<T> Instance = new();
    
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // always going to be a read to the inner converter
        return JsonSerializer.Deserialize<T>(ref reader, (JsonTypeInfo<T>) options.GetTypeInfo(typeof(T)))!;
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (value.IsSpecified)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}