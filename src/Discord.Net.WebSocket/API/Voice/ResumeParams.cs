using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class ResumeParams
    {
        [JsonProperty("server_id")]
        public ulong GuildId { get; set; } 
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
