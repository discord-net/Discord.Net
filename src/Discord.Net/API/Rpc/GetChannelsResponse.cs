using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GetChannelsResponse
    {
        [JsonProperty("channels")]
        public RpcChannel[] Channels { get; set; }
    }
}
