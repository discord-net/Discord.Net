using System;
using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class DateTimeOffsetPropertyConverter : IPropertyConverter<DateTimeOffset>
    {
        public DateTimeOffset ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.GetDateTimeOffset();
        }
        public void WriteJson(JsonWriter writer, DateTimeOffset value)
            => writer.WriteValue(value);
    }
}
