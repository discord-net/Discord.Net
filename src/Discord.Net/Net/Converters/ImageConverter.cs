using Discord.API;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Discord.Net.Converters
{
    public class ImageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Stream) || objectType == typeof(Optional<Stream>);
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Optional<Stream>)
                value = (Optional<Stream>)value;
            var stream = value as Stream;

            byte[] bytes = new byte[stream.Length - stream.Position];
            stream.Read(bytes, 0, bytes.Length);

            string base64 = Convert.ToBase64String(bytes);
            writer.WriteValue($"data:image/jpeg;base64,{base64}");
        }
    }
}
