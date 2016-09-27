using System;

namespace Discord.Audio
{
    /// <summary> Specifies an audio mode for Discord. </summary>
    [Flags]
    public enum AudioMode : byte
    {
        /// <summary> Audio send/receive is disabled. </summary>
        Disabled = 0,
        /// <summary> Audio can only be broadcasted by the client. </summary>
        Outgoing = 1,
        /// <summary> Audio can only be received by the client. </summary>
        Incoming = 2,
        /// <summary> Audio can be sent and received by the client. </summary>
        Both = Outgoing | Incoming
    }
}
