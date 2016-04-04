using System;

namespace Discord
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
