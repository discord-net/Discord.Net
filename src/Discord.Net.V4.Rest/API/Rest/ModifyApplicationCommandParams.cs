using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyApplicationCommandParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    [JsonPropertyName("default_permission")]
    public Optional<bool> DefaultPermission { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }
}
