using System;
using System.Text.Json;

namespace Discord.Serialization.Json.Converters
{
    internal class ImagePropertyConverter : IJsonPropertyConverter<API.Image>
    {
        public API.Image Read(PropertyMap map, object model, ref JsonReader reader, bool isTopLevel)
        {
            if (isTopLevel)
                reader.Read();
            if (reader.ValueType != JsonValueType.String)
                throw new SerializationException("Bad input, expected String");
            return new API.Image(reader.ParseString());
        }
        public void Write(PropertyMap map, object model, ref JsonWriter writer, API.Image value, bool isTopLevel)
        {
            string str;
            if (value.Stream != null)
            {
                byte[] bytes = new byte[value.Stream.Length - value.Stream.Position];
                value.Stream.Read(bytes, 0, bytes.Length);

                string base64 = Convert.ToBase64String(bytes);
                switch (value.StreamFormat)
                {
                    case ImageFormat.Jpeg:
                        str = $"data:image/jpeg;base64,{base64}";
                        break;
                    case ImageFormat.Png:
                        str = $"data:image/png;base64,{base64}";
                        break;
                    case ImageFormat.Gif:
                        str = $"data:image/gif;base64,{base64}";
                        break;
                    case ImageFormat.WebP:
                        str = $"data:image/webp;base64,{base64}";
                        break;
                    default:
                        throw new SerializationException($"Unable to serialize an {nameof(Image)} with a format of {value.StreamFormat}");
                }
            }
            else
                str = value.Hash;

            if (isTopLevel)
                writer.WriteAttribute(map.Key, str);
            else
                writer.WriteValue(str);
        }
    }
}
