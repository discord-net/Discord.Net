using Newtonsoft.Json;

namespace Discord.API
{
    internal class GuildWidget
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
    }
}
