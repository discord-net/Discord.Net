using System;

namespace Discord
{
    public class RoleUpdatedEventArgs : EventArgs
    {
        public Role Before => null;
        public Role After => null;
        public Server Server => null;
    }
}
