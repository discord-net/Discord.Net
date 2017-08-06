using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class Int8PropertyConverter : IPropertyConverter<sbyte>
    {
        public sbyte ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetInt8();
        }
        public void WriteJson(JsonWriter writer, sbyte value)
            => writer.WriteValue(value);
    }
}
