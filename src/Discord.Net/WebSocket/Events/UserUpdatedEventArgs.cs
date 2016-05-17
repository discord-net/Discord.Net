namespace Discord.WebSocket
{
    public class UserUpdatedEventArgs : UserEventArgs
    {
        public IUser Before { get; }
        public IUser After => User;

        public UserUpdatedEventArgs(IUser before, IUser after)
            : base(after)
        {
            Before = before;
        }
    }
}
