#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Role
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public uint Color { get; set; }
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }
        [JsonProperty("mentionable")]
        public bool Mentionable { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("permissions"), Int53]
        public ulong Permissions { get; set; }
        [JsonProperty("managed")]
        public bool Managed { get; set; }
    }
}
