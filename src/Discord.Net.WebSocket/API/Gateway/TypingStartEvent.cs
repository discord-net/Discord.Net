using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class TypingStartEvent
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("member")]
        public GuildMember Member { get; set; }
        [JsonPropertyName("timestamp")]
        public int Timestamp { get; set; }
    }
}
