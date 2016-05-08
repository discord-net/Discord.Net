namespace Discord.API.Rest
{
    public class GetChannelMessagesParams
    {
        public int Limit { get; set; } = DiscordConfig.MaxMessagesPerBatch;
        public Direction RelativeDirection { get; set; } = Direction.Before;
        public ulong? RelativeMessageId { get; set; } = null;
    }
}
