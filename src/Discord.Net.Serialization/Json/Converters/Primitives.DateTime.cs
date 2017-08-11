using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    public class DateTimePropertyConverter : JsonPropertyConverter<DateTime>
    {
        public override DateTime Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTime();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, DateTime value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }

    public class DateTimeOffsetPropertyConverter : JsonPropertyConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTimeOffset();
        }
        public override void Write(PropertyMap map, object model, ref JsonWriter writer, DateTimeOffset value, string key)
        {
            if (key != null)
                writer.WriteAttribute(key, value);
            else
                writer.WriteValue(value);
        }
    }
}
