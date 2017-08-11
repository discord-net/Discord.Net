using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class Int8PropertyConverter : JsonPropertyConverter<sbyte>
    {
        public override sbyte Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt8();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, sbyte value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class Int16PropertyConverter : JsonPropertyConverter<short>
    {
        public override short Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt16();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, short value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class Int32PropertyConverter : JsonPropertyConverter<int>
    {
        public override int Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt32();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, int value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class Int64PropertyConverter : JsonPropertyConverter<long>
    {
        public override long Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.Number && reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected Number or String");
            return reader.ParseInt64();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, long value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value.ToString());
            else
                writer.WriteValue(value.ToString());
        }
    }
}
