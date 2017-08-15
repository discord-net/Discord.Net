using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class UInt8PropertyConverter : JsonPropertyConverter<byte>
    {
        public override byte Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt8();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, byte value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class UInt16PropertyConverter : JsonPropertyConverter<ushort>
    {
        public override ushort Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt16();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, ushort value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class UInt32PropertyConverter : JsonPropertyConverter<uint>
    {
        public override uint Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt32();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, uint value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class UInt64PropertyConverter : JsonPropertyConverter<ulong>
    {
        public override ulong Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt64();
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, ulong value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
