using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public class GuildMembersChunkEvent
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("members")]
        public Member[] Members { get; set; }
    }
}
