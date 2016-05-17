using System;

namespace Discord.WebSocket
{
    public class VoiceChannelEventArgs : EventArgs
    {
        public VoiceChannel Channel { get; }

        public VoiceChannelEventArgs(VoiceChannel channel)
        {
            Channel = channel;
        }
    }
}
