using System;

namespace Discord.Audio
{
    [Flags]
    public enum AudioMode : byte
    {
        Disabled = 0,
        Outgoing = 1,
        Incoming = 2,
        Both = Outgoing | Incoming
    }
}
