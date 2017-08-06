#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class ChannelSubscriptionParams
    {
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
