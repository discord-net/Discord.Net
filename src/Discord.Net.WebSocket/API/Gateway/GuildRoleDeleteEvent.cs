using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildRoleDeleteEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("role_id")]
        public ulong RoleId { get; set; }
    }
}
