using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    internal class SpeakingEvent
    {
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
        [JsonPropertyName("ssrc")]
        public uint Ssrc { get; set; }
        [JsonPropertyName("speaking")]
        public bool Speaking { get; set; }
    }
}
