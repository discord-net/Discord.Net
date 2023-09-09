using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Converters;

internal sealed class DiscordConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => throw new NotImplementedException();
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
}
