using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class CharPropertyConverter : IPropertyConverter<char>
    {
        public char ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.GetChar();
        }
        public void WriteJson(JsonWriter writer, char value)
            => writer.WriteValue(value);
    }
}
