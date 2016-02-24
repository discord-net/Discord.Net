using System;
namespace Discord
{
    public class UserUpdatedEventArgs : EventArgs
    {
        public User Before => null;
        public User After => null;
        public Server Server => null;
    }
}
