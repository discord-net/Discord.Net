using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ResumeParams
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        [JsonPropertyName("seq")]
        public int Sequence { get; set; }
    }
}
