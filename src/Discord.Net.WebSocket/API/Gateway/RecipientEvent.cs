#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class RecipientEvent
    {
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
