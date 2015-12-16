using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class GuildRoleUpdateEvent
    {
		[JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; }
        [JsonProperty("role")]
        public Role Data { get; }
    }
}
