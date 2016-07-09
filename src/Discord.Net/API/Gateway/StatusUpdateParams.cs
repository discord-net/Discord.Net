using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class StatusUpdateParams
    {
        [JsonProperty("idle_since"), Int53]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
