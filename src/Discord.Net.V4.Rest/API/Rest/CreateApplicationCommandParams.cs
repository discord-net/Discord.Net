using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateApplicationCommandParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    [JsonPropertyName("default_permission")]
    public Optional<bool> DefaultPermission { get; set; }

    [JsonPropertyName("name_localizations")]
    public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

    [JsonPropertyName("description_localizations")]
    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

    [JsonPropertyName("dm_permission")]
    public Optional<bool?> DmPermission { get; set; }

    [JsonPropertyName("default_member_permissions")]
    public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    public CreateApplicationCommandParams() { }

    public CreateApplicationCommandParams(string name, string description, ApplicationCommandType type, ApplicationCommandOption[]? options = null,
        IDictionary<string, string>? nameLocalizations = null, IDictionary<string, string>? descriptionLocalizations = null, bool nsfw = false)
    {
        Name = name;
        Description = description;
        Options = options is not null ? Optional.Create(options) : Optional<ApplicationCommandOption[]>.Unspecified;
        Type = type;
        NameLocalizations = nameLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
        DescriptionLocalizations = descriptionLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
        Nsfw = nsfw;
    }
}
