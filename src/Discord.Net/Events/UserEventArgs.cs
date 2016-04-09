using System;

namespace Discord
{
    public class UserEventArgs : EventArgs
    {
        public IUser User { get; }

        public UserEventArgs(IUser user)
        {
            User = user;
        }
    }
}
