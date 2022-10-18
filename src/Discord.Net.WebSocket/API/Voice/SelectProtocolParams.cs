using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    internal class SelectProtocolParams
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
        [JsonPropertyName("data")]
        public UdpProtocolInfo Data { get; set; }
    }
}
