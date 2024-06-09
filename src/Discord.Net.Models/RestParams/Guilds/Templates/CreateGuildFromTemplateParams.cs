using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGuildFromTemplateParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string> Icon { get; set; }
}
