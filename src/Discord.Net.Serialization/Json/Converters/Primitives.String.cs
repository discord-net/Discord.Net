using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class CharPropertyConverter : IJsonPropertyConverter<char>
    {
        public char Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseChar();
        }
        public void Write(JsonWriter writer, char value)
            => writer.WriteValue(value);
    }

    internal class StringPropertyConverter : IJsonPropertyConverter<string>
    {
        public string Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseString();
        }
        public void Write(JsonWriter writer, string value)
            => writer.WriteValue(value);
    }
}
