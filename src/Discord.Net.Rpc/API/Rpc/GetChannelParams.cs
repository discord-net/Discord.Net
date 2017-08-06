#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class GetChannelParams
    {
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
