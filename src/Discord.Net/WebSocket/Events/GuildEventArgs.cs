using System;

namespace Discord.WebSocket
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
