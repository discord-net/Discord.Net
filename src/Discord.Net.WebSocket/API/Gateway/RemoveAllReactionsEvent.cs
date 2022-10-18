using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class RemoveAllReactionsEvent
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("message_id")]
        public ulong MessageId { get; set; }
    }
}
