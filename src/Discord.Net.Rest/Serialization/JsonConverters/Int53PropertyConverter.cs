using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class Int53PropertyConverter : IJsonPropertyConverter<long>
    {
        public long Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt64();
        }
        public void Write(JsonWriter writer, long value)
            => writer.WriteValue(value);
    }
}
