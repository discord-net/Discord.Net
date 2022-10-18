using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildJoinRequestDeleteEvent
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
    }
}
