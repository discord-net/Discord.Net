using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.GatewaySocket
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IdentifyCommand : IWebSocketMessage
    {
        int IWebSocketMessage.OpCode => (int)OpCode.Identify;
        object IWebSocketMessage.Payload => this;

        [JsonProperty("v")]
        public int Version { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("properties")]
        public IReadOnlyDictionary<string, string> Properties { get; set; }
        [JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
        public int LargeThreshold { get; set; }
        [JsonProperty("compress")]
        public bool UseCompression { get; set; }
    }
}
