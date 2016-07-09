using Discord.API;
using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class ImageConverter : JsonConverter
    {
        public static readonly ImageConverter Instance = new ImageConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var image = (Image)value;

            if (image.Stream != null)
            {
                byte[] bytes = new byte[image.Stream.Length - image.Stream.Position];
                image.Stream.Read(bytes, 0, bytes.Length);

                string base64 = Convert.ToBase64String(bytes);
                writer.WriteValue($"data:image/jpeg;base64,{base64}");
            }
            else if (image.Hash != null)
                writer.WriteValue(image.Hash);
        }
    }
}
