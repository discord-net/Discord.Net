using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Discord.Net.Converters
{
    internal class UInt64EntityConverter : JsonConverter
    {
        public static readonly UInt64EntityConverter Instance = new UInt64EntityConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue((value as IEntity<ulong>).Id.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }
    }
}
