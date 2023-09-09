using System.Text.Json.Serialization;

namespace Discord.API;

public class ApplicationCommandOptionChoice
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public object Value { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonPropertyName("name_localized")]
    public Optional<string> NameLocalized { get; set; }
}
