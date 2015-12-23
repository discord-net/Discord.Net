using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class MemberReference
    {
        [JsonProperty("guild_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? GuildId { get; set; }
        [JsonProperty("user")]
        public UserReference User { get; set; }
    }
}
