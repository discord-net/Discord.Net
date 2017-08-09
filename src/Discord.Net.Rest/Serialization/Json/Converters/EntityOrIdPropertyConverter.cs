using Discord.API;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class EntityOrIdPropertyConverter<T> : IJsonPropertyConverter<EntityOrId<T>>
    {
        private readonly IJsonPropertyConverter<T> _innerConverter;

        public EntityOrIdPropertyConverter(IJsonPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public EntityOrId<T> Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Number)
                return new EntityOrId<T>(reader.ParseUInt64());
            return new EntityOrId<T>(_innerConverter.Read(map, model, ref reader, false));
        }

        public void Write(PropertyMap map, object model, ref JsonWriter writer, EntityOrId<T> value, bool isTopLevel)
        {
            if (value.Object != null)
                _innerConverter.Write(map, model, ref writer, value.Object, isTopLevel);
            else
            {
                if (isTopLevel)
                    writer.WriteAttribute(map.Key, value.Id);
                else
                    writer.WriteValue(value.Id);
            }
        }
    }
}
