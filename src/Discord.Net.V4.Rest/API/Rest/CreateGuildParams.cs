using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("region")]
    public Optional<string?> RegionId { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image?> Icon { get; set; }

    [JsonPropertyName("verification_level")]
    public Optional<VerificationLevel> VerificationLevel { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public Optional<DefaultMessageNotifications> DefaultNotifications { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; set; }

    [JsonPropertyName("roles")]
    public Optional<CreateGuildRoleParams[]> Roles { get; set; }

    [JsonPropertyName("channels")]
    public Optional<CreateGuildChannelParams[]> Channels { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public Optional<ulong> AfkChannelId { get; set; }

    [JsonPropertyName("afk_timeout")]
    public Optional<AfkTimeout> AfkTimeout { get; set; }

    [JsonPropertyName("system_channel_id")]
    public Optional<ulong> SystemChannelId { get; set; }

    [JsonPropertyName("system_channel_flags")]
    public Optional<SystemChannelFlags> Icon { get; set; }
}
