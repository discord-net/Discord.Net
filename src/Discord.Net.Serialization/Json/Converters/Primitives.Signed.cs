using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class Int8PropertyConverter : IJsonPropertyConverter<sbyte>
    {
        public sbyte Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt8();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, sbyte value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class Int16PropertyConverter : IJsonPropertyConverter<short>
    {
        public short Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt16();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, short value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class Int32PropertyConverter : IJsonPropertyConverter<int>
    {
        public int Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt32();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, int value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class Int64PropertyConverter : IJsonPropertyConverter<long>
    {
        public long Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt64();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, long value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Utf16Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
