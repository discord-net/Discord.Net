using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildRoleDeleteEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role_id")]
        public ulong RoleId { get; set; }
    }
}
