using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class UInt53PropertyConverter : JsonPropertyConverter<ulong>
    {
        public override ulong Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt64();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, ulong value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }
}
