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

        public override EntityOrId<T> Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Number)
                return new EntityOrId<T>(reader.ParseUInt64());
            return new EntityOrId<T>(_innerConverter.Read(map, model, ref reader, false));
        }

        public override void Write(PropertyMap map, object model, ref JsonWriter writer, EntityOrId<T> value, string key)
        {
            if (value.Object != null)
                _innerConverter.Write(map, model, ref writer, value.Object, key);
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
