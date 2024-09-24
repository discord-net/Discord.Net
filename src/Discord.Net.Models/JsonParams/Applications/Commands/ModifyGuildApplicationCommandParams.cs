using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyGuildApplicationCommandParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }
    
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
    
    [JsonPropertyName("nsfw")]
    public Optional<bool> IsNsfw { get; set; }
}