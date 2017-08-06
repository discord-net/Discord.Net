using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class UInt32PropertyConverter : IPropertyConverter<uint>
    {
        public uint ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetUInt32();
        }
        public void WriteJson(JsonWriter writer, uint value)
            => writer.WriteValue(value);
    }
}
