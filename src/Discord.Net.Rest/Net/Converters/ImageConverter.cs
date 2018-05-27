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

        /// <exception cref="InvalidOperationException">Cannot read from image.</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var image = (Model)value;

            if (image.Stream != null)
            {
                byte[] bytes;
                int length;
                if (image.Stream.CanSeek)
                {
                    bytes = new byte[image.Stream.Length - image.Stream.Position];
                    length = image.Stream.Read(bytes, 0, bytes.Length);
                }
                else
                {
                    var cloneStream = new MemoryStream();
                    image.Stream.CopyTo(cloneStream);
                    bytes = new byte[cloneStream.Length];
                    cloneStream.Position = 0;
                    cloneStream.Read(bytes, 0, bytes.Length);
                    length = (int)cloneStream.Length;
                }

                string base64 = Convert.ToBase64String(bytes, 0, length);
                writer.WriteValue($"data:image/jpeg;base64,{base64}");
            }
            else if (image.Hash != null)
                writer.WriteValue(image.Hash);
        }
    }
}
