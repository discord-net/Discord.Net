using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildBanEvent : User
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
