namespace Discord
{
    internal interface ISocketUser : IUser, IEntity<ulong>
    {
        SocketGlobalUser User { get; }

        ISocketUser Clone();
    }
}
