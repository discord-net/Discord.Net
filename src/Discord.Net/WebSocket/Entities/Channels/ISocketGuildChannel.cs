namespace Discord.WebSocket
{
    internal interface ISocketGuildChannel : ISocketChannel, IGuildChannel
    {
        new SocketGuild Guild { get; }
    }
}
