#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class SelectChannelParams
    {
        [ModelProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [ModelProperty("force")]
        public Optional<bool> Force { get; set; }
    }
}
