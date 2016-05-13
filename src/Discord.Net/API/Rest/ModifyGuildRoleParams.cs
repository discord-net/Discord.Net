using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildRoleParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("permissions")]
        public Optional<uint> Permissions { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
        [JsonProperty("color")]
        public Optional<uint> Color { get; set; }
        [JsonProperty("hoist")]
        public Optional<bool> Hoist { get; set; }
    }
}
