using System.Text.Json.Serialization;

namespace Discord.API;

public class StageInstance
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public string Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public StagePrivacyLevel PrivacyLevel { get; set; }

    [JsonPropertyName("discoverable_disabled")]
    public bool DiscoverableDisabled { get; set; }

    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong? ScheduledEventId { get; set; }
}
