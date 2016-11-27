using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class RemoveAllReactionsEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; }
    }
}
