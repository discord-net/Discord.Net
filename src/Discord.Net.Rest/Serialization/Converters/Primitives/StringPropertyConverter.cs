using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class StringPropertyConverter : IPropertyConverter<string>
    {
        public string ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.GetString();
        }
        public void WriteJson(JsonWriter writer, string value)
            => writer.WriteValue(value);
    }
}
