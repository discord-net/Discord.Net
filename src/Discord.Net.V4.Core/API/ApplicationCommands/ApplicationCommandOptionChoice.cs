using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ApplicationCommandOptionChoice
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }

    [JsonPropertyName("value")]
    public required object Value { get; set; }
}
