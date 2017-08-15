using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class OptionalPropertyConverter<T> : JsonPropertyConverter<Optional<T>>
    {
        private readonly JsonPropertyConverter<T> _innerConverter;

        public OptionalPropertyConverter(JsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override Optional<T> Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
            => new Optional<T>(_innerConverter.Read(serializer, modelMap, propMap, model, ref reader, isTopLevel));

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, Optional<T> value, string key)
        {
            if (value.IsSpecified)
                _innerConverter.Write(serializer, modelMap, propMap, model, ref writer, value.Value, key);
        }
    }
}
