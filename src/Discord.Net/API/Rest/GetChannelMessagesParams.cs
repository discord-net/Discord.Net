namespace Discord.API.Rest
{
    public class GetChannelMessagesParams
    {
        public int Limit { get; set; } = DiscordRestConfig.MaxMessagesPerBatch;
        public Direction RelativeDirection { get; set; } = Direction.Before;

        public Optional<ulong> RelativeMessageId { get; set; }
        public Optional<IMessage> RelativeMessage { set { RelativeMessageId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }
    }
}
