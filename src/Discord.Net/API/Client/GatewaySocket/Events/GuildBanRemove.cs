using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class GuildBanRemoveEvent
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; }
        [JsonProperty("user_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong UserId { get; }
    }
}
