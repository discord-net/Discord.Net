using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class PresenceUpdateParams

    {
        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }
        [JsonPropertyName("since", NullValueHandling = NullValueHandling.Include), Int53]
        public long? IdleSince { get; set; }
        [JsonPropertyName("afk")]
        public bool IsAFK { get; set; }
        [JsonPropertyName("activities")]
        public object[] Activities { get; set; } // TODO, change to interface later
    }
}
