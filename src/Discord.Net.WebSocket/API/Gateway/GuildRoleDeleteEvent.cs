#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildRoleDeleteEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role_id")]
        public ulong RoleId { get; set; }
    }
}
