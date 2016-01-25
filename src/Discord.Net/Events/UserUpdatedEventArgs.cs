using System;
namespace Discord
{
    public class UserUpdatedEventArgs : EventArgs
    {
        public User Before { get; }
        public User After { get; }

        public Server Server => After.Server;

        public UserUpdatedEventArgs(User before, User after)
        {
            Before = before;
            After = after;
        }
    }
}
