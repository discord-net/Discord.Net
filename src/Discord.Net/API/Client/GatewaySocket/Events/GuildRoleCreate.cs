using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public class GuildRoleCreateEvent
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("role")]
        public Role Data { get; set; }
    }
}
