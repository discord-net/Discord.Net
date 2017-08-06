using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class Int32PropertyConverter : IPropertyConverter<int>
    {
        public int ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetInt32();
        }
        public void WriteJson(JsonWriter writer, int value)
            => writer.WriteValue(value);
    }
}
