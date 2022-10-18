using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class Reaction
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        [JsonPropertyName("message_id")]
        public ulong MessageId { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonPropertyName("emoji")]
        public Emoji Emoji { get; set; }
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; set; }
    }
}
