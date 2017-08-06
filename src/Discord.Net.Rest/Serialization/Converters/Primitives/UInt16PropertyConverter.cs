using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class UInt16PropertyConverter : IPropertyConverter<ushort>
    {
        public ushort ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetUInt16();
        }
        public void WriteJson(JsonWriter writer, ushort value)
            => writer.WriteValue(value);
    }
}
