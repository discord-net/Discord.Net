using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class SinglePropertyConverter : IPropertyConverter<float>
    {
        public float ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetSingle();
        }
        public void WriteJson(JsonWriter writer, float value)
            => writer.WriteValue(value.ToString());
    }
}
