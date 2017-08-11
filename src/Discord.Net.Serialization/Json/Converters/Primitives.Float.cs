using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class SinglePropertyConverter : JsonPropertyConverter<float>
    {
        public override float Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseSingle();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, float value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }

    public class DoublePropertyConverter : JsonPropertyConverter<double>
    {
        public override double Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseDouble();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, double value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }

    internal class DecimalPropertyConverter : JsonPropertyConverter<decimal>
    {
        public override decimal Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseDecimal();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, decimal value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
