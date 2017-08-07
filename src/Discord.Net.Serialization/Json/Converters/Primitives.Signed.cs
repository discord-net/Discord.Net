using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class Int8PropertyConverter : IJsonPropertyConverter<sbyte>
    {
        public sbyte Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt8();
        }
        public void Write(JsonWriter writer, sbyte value)
            => writer.WriteValue(value);
    }

    internal class Int16PropertyConverter : IJsonPropertyConverter<short>
    {
        public short Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt16();
        }
        public void Write(JsonWriter writer, short value)
            => writer.WriteValue(value);
    }

    internal class Int32PropertyConverter : IJsonPropertyConverter<int>
    {
        public int Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseInt32();
        }
        public void Write(JsonWriter writer, int value)
            => writer.WriteValue(value);
    }

    internal class Int64PropertyConverter : IJsonPropertyConverter<long>
    {
        public long Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseInt64();
        }
        public void Write(JsonWriter writer, long value)
            => writer.WriteValue(value.ToString());
    }
}
