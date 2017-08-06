#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class RpcConfig
    {
        [ModelProperty("cdn_host")]
        public string CdnHost { get; set; }
        [ModelProperty("api_endpoint")]
        public string ApiEndpoint { get; set; }
        [ModelProperty("environment")]
        public string Environment { get; set; }
    }
}
