using Newtonsoft.Json;

namespace Discord.API
{
    public class MemberReference
    {
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
