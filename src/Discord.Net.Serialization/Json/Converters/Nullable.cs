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

        public override T? Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            return _innerConverter.Read(serializer, modelMap, propMap, model, ref reader, false);
        }

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T? value, string key)
        {
            if (value.HasValue)
                _innerConverter.Write(serializer, modelMap, propMap, model, ref writer, value.Value, key);
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
