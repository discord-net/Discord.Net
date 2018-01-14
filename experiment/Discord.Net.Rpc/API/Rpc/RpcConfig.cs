#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class RpcConfig
    {
        [JsonProperty("cdn_host")]
        public string CdnHost { get; set; }
        [JsonProperty("api_endpoint")]
        public string ApiEndpoint { get; set; }
        [JsonProperty("environment")]
        public string Environment { get; set; }
    }
}
