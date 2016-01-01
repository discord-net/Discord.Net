using System;

namespace Discord
{
    public class ProfileEventArgs : EventArgs
    {
        public Profile Profile { get; }

        public ProfileEventArgs(Profile profile)
        {
            Profile = profile;
        }
    }
}
