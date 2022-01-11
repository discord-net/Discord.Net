using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class PresenceUpdateParams

    {
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
        [JsonProperty("since", NullValueHandling = NullValueHandling.Include), Int53]
        public long? IdleSince { get; set; }
        [JsonProperty("afk")]
        public bool IsAFK { get; set; }
        [JsonProperty("activities")]
        public object[] Activities { get; set; } // TODO, change to interface later
    }
}
