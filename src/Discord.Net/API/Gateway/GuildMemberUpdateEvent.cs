using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildMemberUpdateEvent : GuildMember
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
