using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public abstract class TypeUnionConverter<T, TDelimiter> : JsonConverter<T>
{
    protected abstract bool UseGenericAsDefault { get; }
    protected abstract string UnionDelimiterName { get; }

    protected abstract bool TryGetTypeFromDelimiter(TDelimiter delimiter, [MaybeNullWhen(false)] out Type type);

    public sealed override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var obj = JsonDocument.ParseValue(ref reader).RootElement;

        if (!obj.TryGetProperty(UnionDelimiterName, out var delimiterElement))
            throw new JsonException(
                $"Required property '{UnionDelimiterName}' was not present in the provided json object");

        var delimiter = delimiterElement.Deserialize<TDelimiter>(options) ?? throw new JsonException("Union delimiter was null");

        if (TryGetTypeFromDelimiter(delimiter, out var type))
            return (T?)obj.Deserialize(type, options);

        if (!UseGenericAsDefault)
            throw new JsonException("No union target types found");

        return obj.Deserialize<T>(options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => throw new NotSupportedException();
}
