using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildRoleUpdateEvent
    {
		[JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("role")]
        public Role Role { get; set; }
    }
}
