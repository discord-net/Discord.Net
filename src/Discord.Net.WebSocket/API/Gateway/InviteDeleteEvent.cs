using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class InviteDeleteEvent
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
    }
}
