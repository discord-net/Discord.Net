using Newtonsoft.Json;

using System;
using System.IO;
using Model = Discord.API.Sound;

namespace Discord.Net.Converters;

internal class SoundConverter : JsonConverter
{
    public static readonly SoundConverter Instance = new ();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    /// <exception cref="InvalidOperationException">Cannot read from sound.</exception>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new InvalidOperationException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var sound = (Model)value;

        if (sound.Stream != null)
        {
            byte[] bytes;
            int length;
            if (sound.Stream.CanSeek)
            {
                bytes = new byte[sound.Stream.Length - sound.Stream.Position];
                length = sound.Stream.Read(bytes, 0, bytes.Length);
            }
            else
            {
                using (var cloneStream = new MemoryStream())
                {
                    sound.Stream.CopyTo(cloneStream);
                    bytes = new byte[cloneStream.Length];
                    cloneStream.Position = 0;
                    cloneStream.Read(bytes, 0, bytes.Length);
                    length = (int)cloneStream.Length;
                }
            }

            string base64 = Convert.ToBase64String(bytes, 0, length);
            writer.WriteValue($"data:audio/mp3;base64,{base64}");
        }
        else if (sound.Hash != null)
            writer.WriteValue(sound.Hash);
    }

}

