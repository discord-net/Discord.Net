using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Rest.Converters;

public class OptionalConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType
                                                           && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];

        var converter = (JsonConverter)Activator.CreateInstance(
            typeof(OptionalConverterInner<>)
                .MakeGenericType(elementType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }
}


public class OptionalConverterInner<T> : JsonConverter<Optional<T>>
{
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        writer.WritePropertyName("");
    }

    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Optional<T>(JsonSerializer.Deserialize<T>(ref reader, options)!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
