using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    /*public class CharPropertyConverter : IJsonPropertyConverter<char>
    {
        public char Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseChar();
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, char value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value.ToString());
        }
    }*/

    public class StringPropertyConverter : IJsonPropertyConverter<string>
    {
        public string Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType == JsonValueType.Null)
                return null;
            else if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseString();
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, string value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }
}
