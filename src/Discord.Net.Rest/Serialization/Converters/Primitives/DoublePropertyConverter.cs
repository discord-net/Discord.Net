using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class DoublePropertyConverter : IPropertyConverter<double>
    {
        public double ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetDouble();
        }
        public void WriteJson(JsonWriter writer, double value)
            => writer.WriteValue(value.ToString());
    }
}
