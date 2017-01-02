#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class ReadState
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("mention_count")]
        public int MentionCount { get; set; }
        [JsonProperty("last_message_id")]
        public Optional<ulong> LastMessageId { get; set; }
    }
}
