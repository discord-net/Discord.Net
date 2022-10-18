using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class GuildScheduledEvent
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("channel_id")]
        public Optional<ulong?> ChannelId { get; set; }
        [JsonPropertyName("creator_id")]
        public Optional<ulong> CreatorId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }
        [JsonPropertyName("scheduled_start_time")]
        public DateTimeOffset ScheduledStartTime { get; set; }
        [JsonPropertyName("scheduled_end_time")]
        public DateTimeOffset? ScheduledEndTime { get; set; }
        [JsonPropertyName("privacy_level")]
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; }
        [JsonPropertyName("status")]
        public GuildScheduledEventStatus Status { get; set; }
        [JsonPropertyName("entity_type")]
        public GuildScheduledEventType EntityType { get; set; }
        [JsonPropertyName("entity_id")]
        public ulong? EntityId { get; set; }
        [JsonPropertyName("entity_metadata")]
        public GuildScheduledEventEntityMetadata EntityMetadata { get; set; }
        [JsonPropertyName("creator")]
        public Optional<User> Creator { get; set; }
        [JsonPropertyName("user_count")]
        public Optional<int> UserCount { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
    }
}
