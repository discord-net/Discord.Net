#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Voice
{
    internal class SpeakingEvent
    {
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
        [ModelProperty("ssrc")]
        public uint Ssrc { get; set; }
        [ModelProperty("speaking")]
        public bool Speaking { get; set; }
    }
}
