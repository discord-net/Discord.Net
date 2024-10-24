using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGlobalApplicationCommandParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>?> NameLocalizations { get; set; }
    
    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }
    
    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>?> DescriptionLocalizations { get; set; }
    
    [JsonPropertyName("options")]
    public Optional<IApplicationCommandOptionModel[]> Options { get; set; }
    
    [JsonPropertyName("default_member_permissions")]
    public Optional<string?> DefaultMemberPermissions { get; set; }
    
    [JsonPropertyName("integration_types")]
    public Optional<int[]> IntegrationTypes { get; set; }
    
    [JsonPropertyName("contexts")]
    public Optional<int[]> Contexts { get; set; }
    
    [JsonPropertyName("type")]
    public Optional<int> Type { get; set; }
    
    [JsonPropertyName("nsfw")]
    public Optional<bool> IsNsfw { get; set; }
}