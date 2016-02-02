using System;

namespace Discord
{
    public class RoleUpdatedEventArgs : EventArgs
    {
        public Role Before { get; }
        public Role After { get; }

        public Server Server => After.Server;

        public RoleUpdatedEventArgs(Role before, Role after)
        {
            Before = before;
            After = after;
        }
    }
}
