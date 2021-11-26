using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class RemoveAllReactionsForEmoteEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}
