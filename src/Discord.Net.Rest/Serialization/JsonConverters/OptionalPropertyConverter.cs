using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class OptionalPropertyConverter<T> : IJsonPropertyConverter<Optional<T>>
    {
        private readonly IJsonPropertyConverter<T> _innerConverter;

        public OptionalPropertyConverter(IJsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public Optional<T> Read(JsonReader reader, bool read = true)
            => new Optional<T>(_innerConverter.Read(reader, read));

        public void Write(JsonWriter writer, Optional<T> value)
        {
            if (value.IsSpecified)
                _innerConverter.Write(writer, value.Value);
        }
    }
}
