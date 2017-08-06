using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class OptionalPropertyConverter<T> : IPropertyConverter<Optional<T>>
    {
        private readonly IPropertyConverter<T> _innerConverter;

        public OptionalPropertyConverter(IPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public Optional<T> ReadJson(JsonReader reader, bool read = true)
            => new Optional<T>(_innerConverter.ReadJson(reader, read));

        public void WriteJson(JsonWriter writer, Optional<T> value)
        {
            if (value.IsSpecified)
                _innerConverter.WriteJson(writer, value.Value);
        }
    }
}
