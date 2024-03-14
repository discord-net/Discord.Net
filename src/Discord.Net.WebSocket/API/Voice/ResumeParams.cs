using Newtonsoft.Json;

namespace Discord.API.Voice
{
    public class ResumeParams
    {
        [JsonProperty("server_id")]
        public ulong ServerId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
