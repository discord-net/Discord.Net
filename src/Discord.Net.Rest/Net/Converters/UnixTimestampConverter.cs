using System;
using Newtonsoft.Json;

namespace Discord.Net.Converters
{
    public class UnixTimestampConverter : JsonConverter
    {
        public static readonly UnixTimestampConverter Instance = new UnixTimestampConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Discord doesn't validate if timestamps contain decimals or not
            if (reader.Value is double d)
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(d);
            long offset = (long)reader.Value;
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(offset);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}