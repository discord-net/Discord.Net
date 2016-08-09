#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class Application
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("rpc_origins")]
        public string[] RPCOrigins { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("flags"), Int53]
        public ulong Flags { get; set; }
        [JsonProperty("owner")]
        public User Owner { get; set; }
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
