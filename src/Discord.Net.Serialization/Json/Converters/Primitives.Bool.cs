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
}
