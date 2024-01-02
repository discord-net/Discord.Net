using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateGuildFromTemplateParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image> Icon { get; set; }
}
