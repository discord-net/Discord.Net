using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class UInt8PropertyConverter : IJsonPropertyConverter<byte>
    {
        public byte Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt8();
        }
        public void Write(PropertyMap map, JsonWriter writer, byte value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class UInt16PropertyConverter : IJsonPropertyConverter<ushort>
    {
        public ushort Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt16();
        }
        public void Write(PropertyMap map, JsonWriter writer, ushort value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class UInt32PropertyConverter : IJsonPropertyConverter<uint>
    {
        public uint Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt32();
        }
        public void Write(PropertyMap map, JsonWriter writer, uint value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class UInt64PropertyConverter : IJsonPropertyConverter<ulong>
    {
        public ulong Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseUInt64();
        }
        public void Write(PropertyMap map, JsonWriter writer, ulong value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
