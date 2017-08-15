using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class Int53PropertyConverter : JsonPropertyConverter<long>
    {
        public override long Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt64();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, long value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value.ToString());
        }
    }
}
