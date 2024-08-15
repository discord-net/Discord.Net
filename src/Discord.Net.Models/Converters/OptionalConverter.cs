using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public class OptionalConverter : JsonConverterFactory
{
    public static readonly OptionalConverter Instance = new();
    private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = [];

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];

        return Converters.GetOrAdd(elementType, CreateConverter);
    }

    private static JsonConverter CreateConverter(Type typeToConvert)
    {
        return (JsonConverter)Activator.CreateInstance(
            typeof(OptionalConverter<>).MakeGenericType(typeToConvert),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null
        )!;
    }
}

public sealed class OptionalConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Optional<T>(JsonSerializer.Deserialize<T>(ref reader, options)!);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if(value.IsSpecified)
            JsonSerializer.Serialize(writer, value.Value, options);
    }
}
