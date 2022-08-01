using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class WebhooksUpdatedEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
