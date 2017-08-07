using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class NullablePropertyConverter<T> : IJsonPropertyConverter<T?>
        where T : struct
    {
        private readonly IJsonPropertyConverter<T> _innerConverter;

        public NullablePropertyConverter(IJsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public T? Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            return _innerConverter.Read(reader, false);
        }

        public void Write(JsonWriter writer, T? value)
        {
            if (value.HasValue)
                _innerConverter.Write(writer, value.Value);
            else
                writer.WriteNull();
        }
    }
}
