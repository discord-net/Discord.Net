#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ResumeParams
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public int Sequence { get; set; }
    }
}
