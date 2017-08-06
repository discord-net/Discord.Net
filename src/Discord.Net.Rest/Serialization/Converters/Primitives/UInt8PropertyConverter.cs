using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class UInt8PropertyConverter : IPropertyConverter<byte>
    {
        public byte ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetUInt8();
        }
        public void WriteJson(JsonWriter writer, byte value)
            => writer.WriteValue(value);
    }
}
