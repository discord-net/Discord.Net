using System;
using System.Text.Json;

namespace Discord.Serialization.Converters
{
    internal class DateTimePropertyConverter : IPropertyConverter<DateTime>
    {
        public DateTime ReadJson(JsonReader reader, bool read = true)
        {
            if (read)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return reader.GetDateTime();
        }
        public void WriteJson(JsonWriter writer, DateTime value)
            => writer.WriteValue(value);
    }
}
