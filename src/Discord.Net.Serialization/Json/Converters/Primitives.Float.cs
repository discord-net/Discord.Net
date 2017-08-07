using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class SinglePropertyConverter : IJsonPropertyConverter<float>
    {
        public float Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseSingle();
        }
        public void Write(PropertyMap map, JsonWriter writer, float value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }

    internal class DoublePropertyConverter : IJsonPropertyConverter<double>
    {
        public double Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseDouble();
        }
        public void Write(PropertyMap map, JsonWriter writer, double value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }

    internal class DecimalPropertyConverter : IJsonPropertyConverter<decimal>
    {
        public decimal Read(PropertyMap map, JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseDecimal();
        }
        public void Write(PropertyMap map, JsonWriter writer, decimal value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
