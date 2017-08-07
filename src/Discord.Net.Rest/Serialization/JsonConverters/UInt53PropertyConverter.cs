using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class UInt53PropertyConverter : IJsonPropertyConverter<ulong>
    {
        public ulong Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt64();
        }
        public void Write(JsonWriter writer, ulong value)
            => writer.WriteValue(value);
    }
}
