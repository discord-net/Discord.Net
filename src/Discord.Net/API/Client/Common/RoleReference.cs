using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class RoleReference
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong GuildId { get; set; }
        [JsonProperty("role_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong RoleId { get; set; }
    }
}
