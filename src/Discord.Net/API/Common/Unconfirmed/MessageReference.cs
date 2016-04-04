using Newtonsoft.Json;

namespace Discord.API
{
    public class MessageReference
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("message_id")] //Only used in MESSAGE_ACK
        public ulong MessageId { get { return Id; } set { Id = value; } }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
