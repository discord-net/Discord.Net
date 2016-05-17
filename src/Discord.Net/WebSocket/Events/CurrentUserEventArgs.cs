using System;

namespace Discord.WebSocket
{
    public class CurrentUserEventArgs : EventArgs
    {
        public SelfUser CurrentUser { get; }

        public CurrentUserEventArgs(SelfUser currentUser)
        {
            CurrentUser = currentUser;
        }
    }
}
