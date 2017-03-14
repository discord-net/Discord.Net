#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Voice
{
    internal class SpeakingEvent
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("ssrc")]
        public uint Ssrc { get; set; }
        [JsonProperty("speaking")]
        public bool Speaking { get; set; }
    }
}
