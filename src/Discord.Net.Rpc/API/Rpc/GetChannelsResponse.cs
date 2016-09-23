#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GetChannelsResponse
    {
        [JsonProperty("channels")]
        public RpcChannel[] Channels { get; set; }
    }
}
