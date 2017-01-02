#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class RecipientEvent
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
