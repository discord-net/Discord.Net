#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class SpeakingEvent
    {
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
    }
}
