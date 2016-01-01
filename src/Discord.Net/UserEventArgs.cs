using System;
namespace Discord
{
    public class UserEventArgs : EventArgs
    {
        public User User { get; }

        public Server Server => User.Server;

        public UserEventArgs(User user) { User = user; }
    }
}
