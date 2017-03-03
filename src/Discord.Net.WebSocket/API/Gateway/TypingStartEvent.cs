#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class TypingStartEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
