using Newtonsoft.Json;

namespace Discord.API
{
    internal class MessageReference
    {
        [JsonProperty("message_id")]
        public Optional<ulong> MessageId { get; set; }

        [JsonProperty("channel_id")]
        public Optional<ulong> ChannelId { get; set; } // Optional when sending, always present when receiving

        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonProperty("fail_if_not_exists")]
        public Optional<bool> FailIfNotExists { get; set; }
    }
}
