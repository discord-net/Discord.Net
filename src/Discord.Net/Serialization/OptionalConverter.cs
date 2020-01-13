using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Serialization
{
    // TODO: This does not allow us to omit properties at runtime
    // Need to evaluate which cases need us to omit properties and write a separate converter
    // for those. At this time I can only think of the outgoing REST PATCH requests. Incoming
    // omitted properties will be correctly treated as Optional.Unspecified (the default)
    public class OptionalConverter : JsonConverterFactory
    {
        private class OptionalTypeConverter<T> : JsonConverter<Optional<T>>
        {
            public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return Optional<T>.Unspecified;
                else
                    return new Optional<T>(JsonSerializer.Deserialize<T>(ref reader, options));
            }

            public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
            {
                if (!value.IsSpecified)
                    writer.WriteNullValue();
                else
                    JsonSerializer.Serialize(writer, value.Value, options);
            }
        }

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var innerType = typeToConvert.GetGenericArguments()[0];
            var converterType = typeof(OptionalTypeConverter<>).MakeGenericType(innerType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
