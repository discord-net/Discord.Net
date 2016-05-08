using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildRoleParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("permissions")]
        public uint Permissions { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("color")]
        public uint Color { get; set; }
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }
    }
}
