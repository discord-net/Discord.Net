#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class UdpProtocolInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
