using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class StageInstance
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public required string Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public int PrivacyLevel { get; set; }

    [JsonPropertyName("discoverable_disabled")]
    public bool DiscoverableDisabled { get; set; }

    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong? ScheduledEventId { get; set; }
}
