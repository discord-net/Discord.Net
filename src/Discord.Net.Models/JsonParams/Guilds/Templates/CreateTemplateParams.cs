using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateTemplateParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }
}