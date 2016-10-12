#pragma warning disable CS1591
namespace Discord.API.Rest
{
    public class GetChannelMessagesParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<Direction> RelativeDirection { get; set; }
        public Optional<ulong> RelativeMessageId { get; set; }
    }
}
