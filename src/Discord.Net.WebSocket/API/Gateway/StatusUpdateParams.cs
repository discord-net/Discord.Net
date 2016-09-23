#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class StatusUpdateParams
    {
        [JsonProperty("idle_since"), Int53]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
