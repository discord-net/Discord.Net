using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class UInt64PropertyConverter : IPropertyConverter<ulong>
    {
        public ulong ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.GetUInt64();
        }
        public void WriteJson(JsonWriter writer, ulong value)
            => writer.WriteValue(value.ToString());
    }
}
