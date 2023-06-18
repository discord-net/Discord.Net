using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class TypingStartEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
