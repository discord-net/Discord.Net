namespace Discord
{
    public interface IGuildEmbed : ISnowflakeEntity
    {
        bool IsEnabled { get; }
        ulong? ChannelId { get; }
    }
}
