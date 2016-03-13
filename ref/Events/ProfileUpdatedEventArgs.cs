using System;

namespace Discord
{
    public class ProfileUpdatedEventArgs : EventArgs
    {
        public Profile Before => null;
        public Profile After => null;
    }
}
