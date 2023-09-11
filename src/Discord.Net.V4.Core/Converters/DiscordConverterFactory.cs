using Discord.Rest.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

internal sealed class DiscordConverterFactory : JsonConverterFactory
{
    private static readonly JsonConverter[] _converters = new JsonConverter[]
    {
        EmbedTypeConverter.Instance,
        OptionalConverter.Instance,
        UInt64Converter.Instance,
        UserStatusConverter.Instance
    };

    public override bool CanConvert(Type typeToConvert)
        => _converters.Any(x => x.CanConvert(typeToConvert));

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converters.FirstOrDefault(x => x.CanConvert(typeToConvert));
    }
}
