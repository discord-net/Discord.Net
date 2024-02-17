using Newtonsoft.Json;

using System;
using System.Globalization;

namespace Discord.Net.Converters;

internal class SelectMenuDefaultValueTypeConverter : JsonConverter
{
    public static readonly SelectMenuDefaultValueTypeConverter Instance = new ();

    public override bool CanConvert(Type objectType) => true;
    public override bool CanRead => true;
    public override bool CanWrite => true;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return Enum.TryParse<SelectDefaultValueType>((string)reader.Value, true, out var result)
            ? result
            : null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(((SelectDefaultValueType)value).ToString().ToLower(CultureInfo.InvariantCulture));
    }
}
