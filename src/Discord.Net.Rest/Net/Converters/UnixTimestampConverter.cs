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

        // 1e13 unix ms = year 2286
        // necessary to prevent discord.js from sending values in the e15 and overflowing a DTO
        private const long MaxSaneMs = 1_000_000_000_000_0;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Discord doesn't validate if timestamps contain decimals or not, and they also don't validate if timestamps are reasonably sized
            if (reader.Value is double d && d < MaxSaneMs)
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(d);
            else if (reader.Value is long l && l < MaxSaneMs)
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(l);
            return Optional<DateTimeOffset>.Unspecified;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
