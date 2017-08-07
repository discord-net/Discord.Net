using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ObjectPropertyConverter<T> : IJsonPropertyConverter<T>
        where T : class, new()
    {
        private static readonly ModelMap<T> _map = SerializationFormat.Json.MapModel<T>();
        
        public T Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            var model = new T();

            if ((isTopLevel && !reader.Read()) || reader.TokenType != JsonTokenType.StartObject)
                throw new SerializationException("Bad input, expected StartObject");
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return model;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new SerializationException("Bad input, expected PropertyName");

                string key = reader.ParseString();
                if (_map.PropertiesByKey.TryGetValue(key, out var property))
                    (property as IJsonPropertyMap<T>).Read(model, reader);
                else
                    reader.Skip(); //Unknown property, skip

                if (!reader.Read())
                    throw new SerializationException("Bad input, expected Value");
            }
            throw new SerializationException("Bad input, expected EndObject");
        }
        public void Write(PropertyMap map, JsonWriter writer, T value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteObjectStart(map.Key);
            else
                writer.WriteObjectStart();
            for (int i = 0; i < _map.Properties.Length; i++)
                (_map.Properties[i] as IJsonPropertyMap<T>).Write(value, writer);
            writer.WriteObjectEnd();
        }
    }
}