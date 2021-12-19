using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ModifyGuildScheduledEventParams
    {
        [JsonProperty("channel_id")]
        public Optional<ulong?> ChannelId { get; set; }
        [JsonProperty("entity_metadata")]
        public Optional<GuildScheduledEventEntityMetadata> EntityMetadata { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("privacy_level")]
        public Optional<GuildScheduledEventPrivacyLevel> PrivacyLevel { get; set; }
        [JsonProperty("scheduled_start_time")]
        public Optional<DateTimeOffset> StartTime { get; set; }
        [JsonProperty("scheduled_end_time")]
        public Optional<DateTimeOffset> EndTime { get; set; }
        [JsonProperty("description")]
        public Optional<string> Description { get; set; }
        [JsonProperty("entity_type")]
        public Optional<GuildScheduledEventType> Type { get; set; }
        [JsonProperty("status")]
        public Optional<GuildScheduledEventStatus> Status { get; set; }
    }
}
