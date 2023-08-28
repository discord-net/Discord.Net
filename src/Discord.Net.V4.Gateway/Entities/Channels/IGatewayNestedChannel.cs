namespace Discord.Gateway
{
    public interface IGatewayNestedChannel
    {
        GuildChannelCacheable? Category { get; }
    }
}
