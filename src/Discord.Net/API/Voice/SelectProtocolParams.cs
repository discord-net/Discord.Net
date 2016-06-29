using Newtonsoft.Json;

namespace Discord.API.Voice
{
    public class SelectProtocolParams
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }
}
