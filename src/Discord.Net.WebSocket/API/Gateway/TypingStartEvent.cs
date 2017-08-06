#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class TypingStartEvent
    {
        [ModelProperty("user_id")]
        public ulong UserId { get; set; }
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [ModelProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
