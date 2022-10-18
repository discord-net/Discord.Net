using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildRoleUpdateEvent
    {
		[JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("role")]
        public Role Role { get; set; }
    }
}
