using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class MessageComponentInteractionData : IDiscordInteractionData
    {
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("component_type")]
        public ComponentType ComponentType { get; set; }

        [JsonProperty("values")]
        public Optional<string[]> Values { get; set; }

        [JsonProperty("value")]
        public Optional<string> Value { get; set; }

        [JsonProperty("resolved")]
        public Optional<MessageComponentInteractionDataResolved> Resolved { get; set; }
    }
}
