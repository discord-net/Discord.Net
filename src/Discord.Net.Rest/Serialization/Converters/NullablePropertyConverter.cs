using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class NullablePropertyConverter<T> : IPropertyConverter<T?>
        where T : struct
    {
        private readonly IPropertyConverter<T> _innerConverter;

        public NullablePropertyConverter(IPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public T? ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            return _innerConverter.ReadJson(reader);
        }

        public void WriteJson(JsonWriter writer, T? value)
        {
            if (value.HasValue)
                _innerConverter.WriteJson(writer, value.Value);
            else
                writer.WriteNull();
        }
    }
}
