using System;

namespace Discord.Net.WebSockets
{
    public class BinaryMessageEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public BinaryMessageEventArgs(byte[] data) { Data = data; }
    }
}
