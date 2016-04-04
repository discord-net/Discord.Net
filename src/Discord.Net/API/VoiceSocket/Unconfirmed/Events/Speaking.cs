using Newtonsoft.Json;

namespace Discord.API.VoiceSocket
{
    public class SpeakingEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("speaking")]
        public bool IsSpeaking { get; set; }
    }
}
