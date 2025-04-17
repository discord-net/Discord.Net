using Newtonsoft.Json;

using System.Globalization;
using System;

namespace Discord.Net.Converters;

internal class ColorConverter : JsonConverter
{
    public static readonly ColorConverter Instance = new ();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value is null)
            return null;
        return new Color(uint.Parse(reader.Value.ToString()!.TrimStart('#'), NumberStyles.HexNumber));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue($"#{(uint)value:X}");
    }
}
