using Newtonsoft.Json;

namespace Discord.API
{
    public class RoleReference
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role_id")]
        public ulong RoleId { get; set; }
    }
}
