using System;
using System.IO;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ImagePropertyConverter : JsonPropertyConverter<API.Image>
    {
        public override API.Image Read(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return new API.Image(reader.ParseString());
        }
        public override void Write(Serializer serializer, ModelMap modelMap, PropertyMap propMap, object model, ref JsonWriter writer, API.Image value, string key)
        {
            string str;
            if (value.Stream != null)
            {
                byte[] bytes;
                int length;
                if (value.Stream.CanSeek)
                {
                    bytes = new byte[value.Stream.Length - value.Stream.Position];
                    length = value.Stream.Read(bytes, 0, bytes.Length);
                }
                else
                {
                    var tempStream = new MemoryStream();
                    value.Stream.CopyTo(tempStream);
                    bytes = tempStream.GetBuffer();
                    length = (int)tempStream.Length;
                }

                string base64 = Convert.ToBase64String(bytes, 0, length);
                switch (value.StreamFormat)
                {
                    case ImageFormat.Jpeg: str = $"data:image/jpeg;base64,{base64}"; break;
                    case ImageFormat.Png: str = $"data:image/png;base64,{base64}"; break;
                    case ImageFormat.Gif: str = $"data:image/gif;base64,{base64}"; break;
                    case ImageFormat.WebP: str = $"data:image/webp;base64,{base64}"; break;
                    default:
                        throw new SerializationException($"Unable to serialize an {nameof(Image)} with a format of {value.StreamFormat}");
                }
            }
            else
                str = value.Hash;

            if (key != null)
                writer.WriteAttribute(key, str);
            else
                writer.WriteValue(str);
        }
    }
}
