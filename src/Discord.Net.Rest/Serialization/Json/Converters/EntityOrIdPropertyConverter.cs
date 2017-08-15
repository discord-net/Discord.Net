using Discord.API;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class EntityOrIdPropertyConverter<T> : JsonPropertyConverter<EntityOrId<T>>
    {
        private readonly JsonPropertyConverter<T> _innerConverter;

        public EntityOrIdPropertyConverter(JsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override EntityOrId<T> Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Number)
                return new EntityOrId<T>(reader.ParseUInt64());
            return new EntityOrId<T>(_innerConverter.Read(serializer, modelMap, propMap, model, ref reader, false));
        }

        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, EntityOrId<T> value, string key)
        {
            if (value.Object != null)
                _innerConverter.Write(serializer, modelMap, propMap, model, ref writer, value.Object, key);
            else
            {
                if (key != null)
                    writer.WriteAttribute(key, value.Id);
                else
                    writer.WriteValue(value.Id);
            }
        }
    }
}
