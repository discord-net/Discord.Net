using System;

namespace Discord
{
    public class VoiceDisconnectedEventArgs : DisconnectedEventArgs
    {
        public ulong ServerId { get; }

        public VoiceDisconnectedEventArgs(ulong serverId, bool wasUnexpected, Exception ex)
            : base(wasUnexpected, ex)
        {
            ServerId = serverId;
        }
    }
}
