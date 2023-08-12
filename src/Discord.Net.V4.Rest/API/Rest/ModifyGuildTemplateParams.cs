using System.Text.Json.Serialization;

namespace Discord.Net.V4.Rest.API.Rest;

public class ModifyGuildTemplateParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }
}
