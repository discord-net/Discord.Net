using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildJoinRequestDeleteEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
