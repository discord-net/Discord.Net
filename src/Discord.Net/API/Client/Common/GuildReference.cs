using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class GuildReference
    {
        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
