#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class StatusUpdateParams
    {
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
        [JsonProperty("since"), Int53]
        public long? IdleSince { get; set; }
        [JsonProperty("afk")]
        public bool IsAFK { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
