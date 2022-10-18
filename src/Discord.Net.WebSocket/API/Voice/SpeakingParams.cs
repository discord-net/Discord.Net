using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    internal class SpeakingParams
    {
        [JsonPropertyName("speaking")]
        public bool IsSpeaking { get; set; }
        [JsonPropertyName("delay")]
        public int Delay { get; set; }
    }
}
