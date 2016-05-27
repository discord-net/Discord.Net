using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.API
{
    public class WebSocketMessage
    {
        [JsonProperty("op")]
        public int? Operation { get; set; }
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public uint? Sequence { get; set; }
        [JsonProperty("d")]
        public JToken Payload { get; set; }
    }
}
