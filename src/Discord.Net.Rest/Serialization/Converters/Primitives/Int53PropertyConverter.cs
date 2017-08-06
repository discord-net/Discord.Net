using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class Int53PropertyConverter : IPropertyConverter<long>
    {
        public long ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetInt64();
        }
        public void WriteJson(JsonWriter writer, long value)
            => writer.WriteValue(value);
    }
}
