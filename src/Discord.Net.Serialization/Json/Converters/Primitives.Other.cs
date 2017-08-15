using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class BooleanPropertyConverter : JsonPropertyConverter<bool>
    {
        public override bool Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
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
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, bool value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class GuidPropertyConverter : JsonPropertyConverter<Guid>
    {
        public override Guid Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseGuid();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, Guid value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
