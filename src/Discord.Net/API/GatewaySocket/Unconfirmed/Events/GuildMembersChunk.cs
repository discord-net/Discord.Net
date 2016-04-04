using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    public class GuildMembersChunkEvent
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("members")]
        public ExtendedMember[] Members { get; set; }
    }
}
