#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class GetGatewayResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
