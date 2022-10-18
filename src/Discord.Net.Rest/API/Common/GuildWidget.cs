using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class GuildWidget
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong? ChannelId { get; set; }
    }
}
