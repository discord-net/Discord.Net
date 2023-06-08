using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ModifyAutoModRuleParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("event_type")]
        public Optional<AutoModEventType> EventType { get; set; }

        [JsonProperty("trigger_type")]
        public Optional<AutoModTriggerType> TriggerType { get; set; }

        [JsonProperty("trigger_metadata")]
        public Optional<TriggerMetadata> TriggerMetadata { get; set; }

        [JsonProperty("actions")]
        public Optional<AutoModAction[]> Actions { get; set; }

        [JsonProperty("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonProperty("exempt_roles")]
        public Optional<ulong[]> ExemptRoles { get; set; }

        [JsonProperty("exempt_channels")]
        public Optional<ulong[]> ExemptChannels { get; set; }
    }
}

