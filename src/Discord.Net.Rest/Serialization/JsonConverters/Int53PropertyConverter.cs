using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class Int53PropertyConverter : IJsonPropertyConverter<long>
    {
        public long Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt64();
        }
        public void Write(PropertyMap map, JsonWriter writer, long value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value.ToString());
        }
    }
}
