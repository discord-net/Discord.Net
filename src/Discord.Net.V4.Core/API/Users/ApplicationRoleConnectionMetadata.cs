using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ApplicationRoleConnectionMetadata
{
    [JsonPropertyName("type")]
    public RoleConnectionMetadataType Type { get; set; }

    [JsonPropertyName("key")]
    public required string Key { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }
}
