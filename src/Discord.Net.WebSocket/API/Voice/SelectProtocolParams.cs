#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SelectProtocolParams
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }
}
