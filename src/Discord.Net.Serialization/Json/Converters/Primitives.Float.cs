using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class SinglePropertyConverter : IJsonPropertyConverter<float>
    {
        public float Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseSingle();
        }
        public void Write(JsonWriter writer, float value)
            => writer.WriteValue(value.ToString());
    }

    internal class DoublePropertyConverter : IJsonPropertyConverter<double>
    {
        public double Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseDouble();
        }
        public void Write(JsonWriter writer, double value)
            => writer.WriteValue(value.ToString());
    }

    internal class DecimalPropertyConverter : IJsonPropertyConverter<decimal>
    {
        public decimal Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number)
                throw new SerializationException("Bad input, expected Number");
            return reader.ParseDecimal();
        }
        public void Write(JsonWriter writer, decimal value)
            => writer.WriteValue(value.ToString());
    }
}
