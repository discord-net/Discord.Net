using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class DateTimePropertyConverter : IJsonPropertyConverter<DateTime>
    {
        public DateTime Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTime();
        }
        public void Write(JsonWriter writer, DateTime value)
            => writer.WriteValue(value);
    }

    internal class DateTimeOffsetPropertyConverter : IJsonPropertyConverter<DateTimeOffset>
    {
        public DateTimeOffset Read(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.ParseDateTimeOffset();
        }
        public void Write(JsonWriter writer, DateTimeOffset value)
            => writer.WriteValue(value);
    }
}
