using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildTemplateParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }
}
