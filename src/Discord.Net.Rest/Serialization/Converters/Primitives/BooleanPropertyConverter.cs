using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class BooleanPropertyConverter : IPropertyConverter<bool>
    {
        public bool ReadJson(JsonReader reader, bool read = true)
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
        public void WriteJson(JsonWriter writer, bool value)
            => writer.WriteValue(value);
    }
}
