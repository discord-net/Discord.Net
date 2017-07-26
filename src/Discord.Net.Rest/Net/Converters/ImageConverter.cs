using System;
using System.IO;
using Newtonsoft.Json;
using Model = Discord.API.Image;

namespace Discord.Net.Converters
{
    internal class ImageConverter : JsonConverter
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
            var image = (Model)value;

            if (image.Stream != null)
            {
                Stream cloneStream = new MemoryStream();
                image.Stream.CopyTo(cloneStream);

                byte[] bytes = new byte[cloneStream.Length];
                cloneStream.Seek(0, SeekOrigin.Begin);
                cloneStream.Read(bytes, 0, bytes.Length);

                string base64 = Convert.ToBase64String(bytes);
                writer.WriteValue($"data:image/jpeg;base64,{base64}");
            }
            else if (image.Hash != null)
                writer.WriteValue(image.Hash);
        }
    }
}
