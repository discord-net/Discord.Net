using Newtonsoft.Json;

namespace Discord.API
{
    internal class ActionMetadata
    {
        [JsonProperty("channel_id")]
        public Optional<ulong> ChannelId { get; set; }

        [JsonProperty("duration_seconds")]
        public Optional<int> DurationSeconds { get; set; }

        [JsonProperty("custom_message")]
        public Optional<string> CustomMessage { get; set; }
    }
}
