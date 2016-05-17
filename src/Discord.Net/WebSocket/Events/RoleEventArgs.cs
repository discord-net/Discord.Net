using System;

namespace Discord.WebSocket
{
    public class RoleEventArgs : EventArgs
    {
        public Role Role { get; }

        public RoleEventArgs(Role role)
        {
            Role = role;
        }
    }
}
