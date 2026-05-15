using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SpeakingParams
    {
        [JsonProperty("speaking")]
        public int Speaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
        [JsonProperty("ssrc")]
        public uint Ssrc { get; set; }
    }
}
