using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class UInt53PropertyConverter : IJsonPropertyConverter<ulong>
    {
        public ulong Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt64();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, ulong value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value);
            else
                writer.WriteValue(value.ToString());
        }
    }
}
