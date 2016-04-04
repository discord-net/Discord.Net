using System;

namespace Discord
{
    public class GuildEventArgs : EventArgs
    {
        public Guild Guild { get; }

        public GuildEventArgs(Guild guild)
        {
            Guild = guild;
        }
    }
}
