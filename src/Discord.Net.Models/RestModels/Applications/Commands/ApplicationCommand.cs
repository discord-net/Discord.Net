using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(Type))]
public class ApplicationCommand : IApplicationCommandModel
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

    [JsonPropertyName("default_member_permissions")]
    public string? DefaultMemberPermissions { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> IsNsfw { get; set; }

    [JsonPropertyName("integration_types")]
    public Optional<int[]> IntegrationTypes { get; set; }
    
    [JsonPropertyName("contexts")]
    public Optional<int[]?> Contexts { get; set; }
    
    [JsonPropertyName("version")]
    public ulong Version { get; set; }
    
    ulong? IApplicationCommandModel.GuildId => GuildId.ToNullable();
    IReadOnlyDictionary<string, string>? IApplicationCommandModel.NameLocalization => ~NameLocalizations;
    IReadOnlyDictionary<string, string>? IApplicationCommandModel.DescriptionLocalization => ~DescriptionLocalizations;
    bool? IApplicationCommandModel.IsNsfw => IsNsfw.ToNullable();
    int[]? IApplicationCommandModel.IntegrationTypes => ~IntegrationTypes;
    int[]? IApplicationCommandModel.Contexts => ~Contexts;
}
