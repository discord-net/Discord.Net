#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class GuildEmbed
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
