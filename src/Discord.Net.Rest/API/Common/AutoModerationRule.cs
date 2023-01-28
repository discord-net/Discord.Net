using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class AutoModerationRule
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("creator_id")]
        public ulong CreatorId { get; set; }

        [JsonProperty("event_type")]
        public AutoModEventType EventType { get; set; }

        [JsonProperty("trigger_type")]
        public AutoModTriggerType TriggerType { get; set; }

        [JsonProperty("trigger_metadata")]
        public TriggerMetadata TriggerMetadata { get; set; }

        [JsonProperty("actions")]
        public AutoModAction[] Actions { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("exempt_roles")]
        public ulong[] ExemptRoles { get; set; }

        [JsonProperty("exempt_channels")]
        public ulong[] ExemptChannels { get; set; }
    }
}
