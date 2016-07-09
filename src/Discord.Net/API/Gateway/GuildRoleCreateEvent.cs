using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildRoleCreateEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role")]
        public Role Role { get; set; }
    }
}
