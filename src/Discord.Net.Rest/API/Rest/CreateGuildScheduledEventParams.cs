using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class CreateGuildScheduledEventParams
    {
        [JsonPropertyName("channel_id")]
        public Optional<ulong> ChannelId { get; set; }
        [JsonPropertyName("entity_metadata")]
        public Optional<GuildScheduledEventEntityMetadata> EntityMetadata { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("privacy_level")]
        public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; }
        [JsonPropertyName("scheduled_start_time")]
        public DateTimeOffset StartTime { get; set; }
        [JsonPropertyName("scheduled_end_time")]
        public Optional<DateTimeOffset> EndTime { get; set; }
        [JsonPropertyName("description")]
        public Optional<string> Description { get; set; }
        [JsonPropertyName("entity_type")]
        public GuildScheduledEventType Type { get; set; }
        [JsonPropertyName("image")]
        public Optional<Image> Image { get; set; }
    }
}
