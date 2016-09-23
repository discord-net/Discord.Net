#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class Application
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("rpc_origins")]
        public string[] RpcOrigins { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
