using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class Reaction
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}
