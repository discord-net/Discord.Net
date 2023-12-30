using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

public sealed class DiscordConverterFactory : JsonConverterFactory
{
    public static readonly DiscordConverterFactory Instance = new();

    private static readonly JsonConverter[] _converters =
    [
        EmbedTypeConverter.Instance,
        OptionalConverter.Instance,
        UInt64Converter.Instance,
        UserStatusConverter.Instance,
        ColorConverter.Instance
    ];

    public override bool CanConvert(Type typeToConvert)
        => _converters.Any(x => x.CanConvert(typeToConvert));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converters.FirstOrDefault(x => x.CanConvert(typeToConvert));
    }
}
