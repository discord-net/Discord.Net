using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildBanEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}
