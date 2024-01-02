using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateGuildScheduledEventParams
{
    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("entity_metadata")]
    public Optional<GuildScheduledEventEntityMetadata> EntityMetadata { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("privacy_level")]
    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; }

    [JsonPropertyName("scheduled_start_time")]
    public DateTimeOffset ScheduledStartTime { get; set; }

    [JsonPropertyName("scheduled_end_time")]
    public Optional<DateTimeOffset> ScheduledEndTime { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("entity_type")]
    public Optional<GuildScheduledEntityType> EntityType { get; set; }

    [JsonPropertyName("image")]
    public Optional<Image> Image { get; set; }
}
