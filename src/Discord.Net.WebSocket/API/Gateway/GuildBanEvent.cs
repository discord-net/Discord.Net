#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildBanEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
