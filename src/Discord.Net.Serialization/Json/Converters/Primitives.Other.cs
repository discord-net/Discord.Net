using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class BooleanPropertyConverter : IJsonPropertyConverter<bool>
    {
        public bool Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            switch (reader.ValueType)
            {
                case JsonValueType.True: return true;
                case JsonValueType.False: return false;
                default: throw new SerializationException("Bad input, expected False or True");
            }
        }
        public void Write(JsonWriter writer, bool value)
            => writer.WriteValue(value);
    }

    internal class GuidPropertyConverter : IJsonPropertyConverter<Guid>
    {
        public Guid Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return Guid.Parse(reader.ParseString());
        }
        public void Write(JsonWriter writer, Guid value)
            => writer.WriteValue(value);
    }
}
