namespace Discord
{
    internal interface ISocketGuildChannel : ISocketChannel, IGuildChannel
    {
        new SocketGuild Guild { get; }
    }
}
