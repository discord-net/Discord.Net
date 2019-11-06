using Newtonsoft.Json;

namespace Discord.API
{
    internal class MessageReference
    {
        [JsonProperty("message_id")]
        public ulong? MessageId { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
    }
}
