using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class CreateAutoModRuleParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("event_type")]
        public AutoModEventType EventType { get; set; }

        [JsonProperty("trigger_type")]
        public AutoModTriggerType TriggerType { get; set; }

        [JsonProperty("trigger_metadata")]
        public Optional<TriggerMetadata> TriggerMetadata { get; set; }

        [JsonProperty("actions")]
        public AutoModAction[] Actions { get; set; }

        [JsonProperty("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonProperty("exempt_roles")]
        public Optional<ulong[]> ExemptRoles { get; set; }

        [JsonProperty("exempt_channels")]
        public Optional<ulong[]> ExemptChannels { get; set; }
    }
}
