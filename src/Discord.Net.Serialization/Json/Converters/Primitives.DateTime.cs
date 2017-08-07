using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class DateTimePropertyConverter : IJsonPropertyConverter<DateTime>
    {
        public DateTime Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTime();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, DateTime value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }

    internal class DateTimeOffsetPropertyConverter : IJsonPropertyConverter<DateTimeOffset>
    {
        public DateTimeOffset Read(PropertyMap map, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTimeOffset();
        }
        public void Write(PropertyMap map, ref JsonWriter writer, DateTimeOffset value, bool isTopLevel)
        {
            if (isTopLevel)
                writer.WriteAttribute(map.Key, value);
            else
                writer.WriteValue(value);
        }
    }
}
