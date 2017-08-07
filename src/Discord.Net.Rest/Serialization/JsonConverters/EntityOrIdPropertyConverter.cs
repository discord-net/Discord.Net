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

        public EntityOrId<T> Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType == JsonValueType.Number)
                return new EntityOrId<T>(reader.ParseUInt64());
            return new EntityOrId<T>(_innerConverter.Read(reader));
        }

        public void Write(JsonWriter writer, EntityOrId<T> value)
        {
            if (value.Object != null)
                _innerConverter.Write(writer, value.Object);
            else
                writer.WriteValue(value.Id);
        }
    }
}
