using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Discord.Net.Serialization.Converters
{
    /// <summary>
    /// Defines a converter which can be used to convert instances of
    /// <see cref="Optional{T}"/>.
    /// </summary>
    public sealed class OptionalConverter<T> : JsonConverter<T>
    {
        private readonly JsonConverter<T>? _valueConverter;

        internal OptionalConverter(
            JsonSerializerOptions options)
        {
            _valueConverter = options.GetConverter(typeof(T))
                as JsonConverter<T>;
        }

        /// <inheritdoc/>
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            return _valueConverter != null
                ? _valueConverter.Read(ref reader, typeof(T), options)
                : JsonSerializer.Deserialize<T>(ref reader, options);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value,
            JsonSerializerOptions options)
        {
            if (_valueConverter != null)
            {
                _valueConverter.Write(writer, value, options);
                return;
            }

            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
