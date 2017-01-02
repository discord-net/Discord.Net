#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class GetBotGatewayResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("shards")]
        public int Shards { get; set; }
    }
}
