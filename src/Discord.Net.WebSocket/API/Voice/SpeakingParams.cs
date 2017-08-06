#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class SpeakingParams
    {
        [ModelProperty("speaking")]
        public bool IsSpeaking { get; set; }
        [ModelProperty("delay")]
        public int Delay { get; set; }
    }
}
