using System;

namespace Discord
{
    internal class InternalFrameEventArgs : EventArgs
    {
        public ulong UserId { get; }
        public ulong ChannelId { get; }
        public byte[] Buffer { get; }
        public int Offset { get; }
        public int Count { get; }

        public InternalFrameEventArgs(ulong userId, ulong channelId, byte[] buffer, int offset, int count)
        {
            UserId = userId;
            ChannelId = channelId;
            Buffer = buffer;
            Offset = offset;
            Count = count;
        }
    }
}
