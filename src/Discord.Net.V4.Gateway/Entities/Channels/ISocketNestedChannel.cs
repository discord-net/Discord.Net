namespace Discord.Gateway
{
    public interface ISocketNestedChannel
    {
        GuildChannelCacheable Parent { get; }
    }
}
