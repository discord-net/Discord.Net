#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class ReadyEvent
    {
        [JsonProperty("v")]
        public int Version { get; set; }
        [JsonProperty("config")]
        public RpcConfig Config { get; set; }
    }
}
