using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class RemoveAllReactionsEvent
    {
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [ModelProperty("message_id")]
        public ulong MessageId { get; set; }
    }
}
