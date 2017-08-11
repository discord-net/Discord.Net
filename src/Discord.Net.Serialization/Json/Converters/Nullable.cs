using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class NullablePropertyConverter<T> : JsonPropertyConverter<T?>
        where T : struct
    {
        private readonly JsonPropertyConverter<T> _innerConverter;

        public NullablePropertyConverter(JsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override T? Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            return _innerConverter.Read(map, model, ref reader, false);
        }

        public override void Write(PropertyMap map, object model, ref JsonWriter writer, T? value, string key)
        {
            if (value.HasValue)
                _innerConverter.Write(map, model, ref writer, value.Value, key);
            else
            {
                if (key != null)
                    writer.WriteAttributeNull(key);
                else
                    writer.WriteNull();
            }
        }
    }
}
