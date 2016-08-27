#pragma warning disable CS1591
namespace Discord.API.Rest
{
    public class GetChannelMessagesParams
    {
        public int Limit { internal get; set; } = DiscordConfig.MaxMessagesPerBatch;

        public Direction RelativeDirection { internal get; set; } = Direction.Before;

        internal Optional<ulong> _relativeMessageId { get; set; }
        public ulong RelativeMessageId { set { _relativeMessageId = value; } }
        public IMessage RelativeMessage { set { _relativeMessageId = value.Id; } }
    }
}
