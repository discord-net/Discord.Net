using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class ResumeCommand
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public uint Sequence { get; set; }
    }
}
