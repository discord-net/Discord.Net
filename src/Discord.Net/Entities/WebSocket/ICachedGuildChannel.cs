namespace Discord
{
    internal interface ICachedGuildChannel : ICachedChannel, IGuildChannel
    {
        new CachedGuild Guild { get; }
    }
}
