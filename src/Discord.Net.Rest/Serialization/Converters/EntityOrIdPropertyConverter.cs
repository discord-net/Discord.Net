using Discord.API;
using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class EntityOrIdPropertyConverter<T> : IPropertyConverter<EntityOrId<T>>
    {
        private readonly IPropertyConverter<T> _innerConverter;

        public EntityOrIdPropertyConverter(IPropertyConverter<T> innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public EntityOrId<T> ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType == JsonValueType.Number)
                return new EntityOrId<T>(reader.GetUInt64());
            return new EntityOrId<T>(_innerConverter.ReadJson(reader));
        }

        public void WriteJson(JsonWriter writer, EntityOrId<T> value)
        {
            if (value.Object != null)
                _innerConverter.WriteJson(writer, value.Object);
            else
                writer.WriteValue(value.Id);
        }
    }
}
