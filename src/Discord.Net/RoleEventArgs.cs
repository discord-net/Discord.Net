using System;

namespace Discord
{
    public class RoleEventArgs : EventArgs
    {
        public Role Role { get; }

        public Server Server => Role.Server;

        public RoleEventArgs(Role role) { Role = role; }
    }
}
