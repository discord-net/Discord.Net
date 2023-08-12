using System.Text.Json.Serialization;

namespace Discord.API;

internal class GuildTemplate
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; set; }

    [JsonPropertyName("creator_id")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("creator")]
    public User Creator { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("source_guild_id")]
    public ulong SourceGuildId { get; set; }

    [JsonPropertyName("serialized_source_guild")]
    public PartialGuild SourceGuild { get; set; }

    [JsonPropertyName("is_dirty")]
    public bool? IsOutOfSync { get; set; }
}
