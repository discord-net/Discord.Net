namespace Discord.WebSocket
{
    internal interface ISocketUser : IUser, IEntity<ulong>
    {
        SocketGlobalUser User { get; }

        ISocketUser Clone();
    }
}
