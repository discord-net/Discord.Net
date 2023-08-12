using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildFromTemplateParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image> Icon { get; set; }
}
