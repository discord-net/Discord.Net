using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildRoleUpdateEvent
    {
		[JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role")]
        public Role Role { get; set; }
    }
}
