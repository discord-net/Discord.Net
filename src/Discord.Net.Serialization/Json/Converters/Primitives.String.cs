using System.Text.Json;
using System.Text.Utf8;

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
                writer.WriteAttribute(map.Utf16Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class Utf8StringPropertyConverter : IJsonPropertyConverter<Utf8String>
    {
        public Utf8String Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return new Utf8String(reader.Value);
        }
        public void Write(PropertyMap map, ref JsonWriter writer, Utf8String value, bool isTopLevel)
        {
            //TODO: Serialization causes allocs, fix
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
