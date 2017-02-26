namespace Discord.Audio
{
    public struct RTPFrame
    {
        public readonly ushort Sequence;
        public readonly uint Timestamp;
        public readonly byte[] Payload;

        public RTPFrame(ushort sequence, uint timestamp, byte[] payload)
        {
            Sequence = sequence;
            Timestamp = timestamp;
            Payload = payload;
        }
    }
}