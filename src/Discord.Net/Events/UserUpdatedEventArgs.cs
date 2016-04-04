namespace Discord
{
    public class UserUpdatedEventArgs : UserEventArgs
    {
        public User Before { get; }
        public User After => User;

        public UserUpdatedEventArgs(User before, User after)
            : base(after)
        {
            Before = before;
        }
    }
}
