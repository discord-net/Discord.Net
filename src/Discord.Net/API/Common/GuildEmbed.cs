#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class GuildEmbed
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
