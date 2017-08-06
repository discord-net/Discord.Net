using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class Int16PropertyConverter : IPropertyConverter<short>
    {
        public short ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetInt16();
        }
        public void WriteJson(JsonWriter writer, short value)
            => writer.WriteValue(value);
    }
}
