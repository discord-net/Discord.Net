using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    /*internal class CharPropertyConverter : IJsonPropertyConverter<char>
    {
        public char Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseChar();
        }
        public void Write(PropertyMap map, JsonWriter writer, char value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value.ToString());
        }
    }*/

    internal class StringPropertyConverter : IJsonPropertyConverter<string>
    {
        public string Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseString();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, string value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }
}
