namespace Discord.API.Rest
{
    internal class GetChannelMessagesParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<Direction> RelativeDirection { get; set; }
        public Optional<ulong> RelativeMessageId { get; set; }
    }
}
