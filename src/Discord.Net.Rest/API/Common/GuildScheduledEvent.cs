using Newtonsoft.Json;

using System;

namespace Discord.API;

internal class GuildScheduledEvent
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    [JsonProperty("creator_id")]
    public Optional<ulong?> CreatorId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("scheduled_start_time")]
    public DateTimeOffset ScheduledStartTime { get; set; }

    [JsonProperty("scheduled_end_time")]
    public DateTimeOffset? ScheduledEndTime { get; set; }

    [JsonProperty("privacy_level")]
    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; }

    [JsonProperty("status")]
    public GuildScheduledEventStatus Status { get; set; }

    [JsonProperty("entity_type")]
    public GuildScheduledEventType EntityType { get; set; }

    [JsonProperty("entity_id")]
    public ulong? EntityId { get; set; }
    [JsonProperty("entity_metadata")]
    public GuildScheduledEventEntityMetadata EntityMetadata { get; set; }

    [JsonProperty("creator")]
    public Optional<User> Creator { get; set; }

    [JsonProperty("user_count")]
    public Optional<int> UserCount { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonProperty("recurrence_rule")]
    public GuildScheduledEventRecurrenceRule RecurrenceRule { get; set; }
}
