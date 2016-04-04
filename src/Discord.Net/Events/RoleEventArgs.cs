using System;

namespace Discord
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
