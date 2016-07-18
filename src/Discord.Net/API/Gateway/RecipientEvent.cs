using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class RecipientEvent
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
