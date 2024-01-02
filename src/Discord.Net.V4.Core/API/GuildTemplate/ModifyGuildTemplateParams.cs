using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildTemplateParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }
}
