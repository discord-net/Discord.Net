using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class ObjectPropertyConverter<T> : JsonPropertyConverter<T>
        where T : class, new()
    {
        private readonly ModelMap _map;

        public ObjectPropertyConverter(Serializer serializer)
        {
            _map = serializer.MapModel<T>();
        }

        public override T Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            var subModel = new T();

            if ((isTopLevel && !reader.Read()) || (reader.TokenType != JsonTokenType.StartObject && reader.ValueType != JsonValueType.Null))
                throw new SerializationException("Bad input, expected StartObject or Null");

            if (reader.ValueType == JsonValueType.Null)
                return null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return subModel;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new SerializationException("Bad input, expected PropertyName");

                if (_map.TryGetProperty(reader.Value, out var property))
                {
                    try { (property as IJsonPropertyMap<T>).Read(serializer, subModel, ref reader); }
                    catch (Exception ex) { RaiseModelError(serializer, property, ex); }
                }
                else
                {
                    RaiseUnmappedProperty(serializer, _map, reader.Value);
                    JsonReaderUtils.Skip(ref reader); //Unknown property, skip
                }
            }
            throw new SerializationException("Bad input, expected EndObject");
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, T value, string key)
        {
            if (value == null)
            {
                if (key != null)
                    writer.WriteAttributeNull(key);
                else
                    writer.WriteNull();
            }
            else
            {
                if (key != null)
                    writer.WriteObjectStart(key);
                else
                    writer.WriteObjectStart();
                for (int i = 0; i < _map.Properties.Count; i++)
                    (_map.Properties[i] as IJsonPropertyMap<T>).Write(serializer, value, ref writer);
                writer.WriteObjectEnd();
            }
        }
    }
}