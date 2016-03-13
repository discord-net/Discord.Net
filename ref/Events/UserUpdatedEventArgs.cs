using System;
namespace Discord
{
    public class UserUpdatedEventArgs : EventArgs
    {
        public IUser Before => null;
        public IUser After => null;
    }
}
