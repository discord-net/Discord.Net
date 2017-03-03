#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SpeakingParams
    {
        [JsonProperty("speaking")]
        public bool IsSpeaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
