using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class WebhooksUpdatedEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
