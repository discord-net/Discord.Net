using System;

namespace Discord
{
    public class ProfileUpdatedEventArgs : EventArgs
    {
        public Profile Before { get; }
        public Profile After { get; }

        public ProfileUpdatedEventArgs(Profile before, Profile after)
        {
            Before = before;
            After = after;
        }
    }
}
