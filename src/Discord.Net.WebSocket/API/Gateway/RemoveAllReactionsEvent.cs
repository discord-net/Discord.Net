using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class RemoveAllReactionsEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; }
    }
}
