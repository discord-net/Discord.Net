#pragma warning disable CS1591
using Discord.Serialization;
namespace Discord.API.Rpc
{
    internal class MessageEvent
    {
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [ModelProperty("message")]
        public Message Message { get; set; }
    }
}
