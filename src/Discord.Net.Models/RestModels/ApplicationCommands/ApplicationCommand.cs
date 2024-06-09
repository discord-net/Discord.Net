using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommand
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int? Type { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public ulong? DefaultMemberPermission { get; set; }

    [JsonPropertyName("dm_permission")]
    public Optional<bool> DmPermission { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> IsNsfw { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }
}
