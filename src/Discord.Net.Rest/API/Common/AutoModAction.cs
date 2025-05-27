using Newtonsoft.Json;

namespace Discord.API
{
    internal class AutoModAction
    {
        [JsonProperty("type")]
        public AutoModActionType Type { get; set; }

        [JsonProperty("metadata")]
        public Optional<ActionMetadata> Metadata { get; set; }
    }
}
