using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class Role
    {
        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("permissions")]
        public uint? Permissions { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public int? Position { get; set; }
        [JsonProperty("hoist")]
        public bool? Hoist { get; set; }
        [JsonProperty("color")]
        public uint? Color { get; set; }
        [JsonProperty("managed")]
        public bool? Managed { get; set; }
        [JsonProperty("mentionable")]
        public bool? Mentionable { get; set; }
    }
}
