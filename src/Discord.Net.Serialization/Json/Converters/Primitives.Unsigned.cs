using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class UInt8PropertyConverter : IJsonPropertyConverter<byte>
    {
        public byte Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt8();
        }
        public void Write(JsonWriter writer, byte value)
            => writer.WriteValue(value);
    }

    internal class UInt16PropertyConverter : IJsonPropertyConverter<ushort>
    {
        public ushort Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt16();
        }
        public void Write(JsonWriter writer, ushort value)
            => writer.WriteValue(value);
    }

    internal class UInt32PropertyConverter : IJsonPropertyConverter<uint>
    {
        public uint Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseUInt32();
        }
        public void Write(JsonWriter writer, uint value)
            => writer.WriteValue(value);
    }

    internal class UInt64PropertyConverter : IJsonPropertyConverter<ulong>
    {
        public ulong Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseUInt64();
        }
        public void Write(JsonWriter writer, ulong value)
            => writer.WriteValue(value.ToString());
    }
}
