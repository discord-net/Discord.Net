using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class DecimalPropertyConverter : IPropertyConverter<decimal>
    {
        public decimal ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.GetDecimal();
        }
        public void WriteJson(JsonWriter writer, decimal value)
            => writer.WriteValue(value.ToString());
    }
}
