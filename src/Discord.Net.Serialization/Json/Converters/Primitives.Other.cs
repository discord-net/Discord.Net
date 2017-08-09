using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class BooleanPropertyConverter : IJsonPropertyConverter<bool>
    {
        public bool Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            switch (reader.ValueType)
            {
                case JsonValueType.True: return true;
                case JsonValueType.False: return false;
                default: throw new SerializationException("Bad input, expected False or True");
            }
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, bool value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class GuidPropertyConverter : IJsonPropertyConverter<Guid>
    {
        public Guid Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseGuid();
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, Guid value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
