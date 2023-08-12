using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildScheduledEventParams
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    [JsonPropertyName("entity_metadata")]
    public Optional<GuildScheduledEventEntityMetadata?> EntityMetadata { get; set; }

    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("privacy_level")]
    public Optional<GuildScheduledEventPrivacyLevel> PrivacyLevel { get; set; }

    [JsonPropertyName("scheduled_start_time")]
    public Optional<DateTimeOffset> StartTime { get; set; }

    [JsonPropertyName("scheduled_end_time")]
    public Optional<DateTimeOffset> EndTime { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    [JsonPropertyName("entity_type")]
    public Optional<GuildScheduledEventType> Type { get; set; }

    [JsonPropertyName("status")]
    public Optional<GuildScheduledEventStatus> Status { get; set; }

    [JsonPropertyName("image")]
    public Optional<Image?> Image { get; set; }
}
