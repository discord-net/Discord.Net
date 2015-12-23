using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class ChannelReference
    {
        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("guild_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? GuildId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
