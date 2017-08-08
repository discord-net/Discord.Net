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

        public T? Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            return _innerConverter.Read(map, ref reader, false);
        }

        public void Write(PropertyMap map, ref JsonWriter writer, T? value, bool isTopLevel)
        {
            if (value.HasValue)
                _innerConverter.Write(map, ref writer, value.Value, isTopLevel);
            else
            {
                if (isTopLevel)
                    writer.WriteAttributeNull(map.Key);
                else
                    writer.WriteNull();
            }
        }
    }
}
