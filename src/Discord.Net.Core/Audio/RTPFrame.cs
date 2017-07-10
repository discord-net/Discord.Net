namespace Discord.Audio
{
    public struct RTPFrame
    {
        public readonly ushort Sequence;
        public readonly uint Timestamp;
        public readonly byte[] Payload;
        public readonly bool Missed;

        public RTPFrame(ushort sequence, uint timestamp, byte[] payload, bool missed)
        {
            Sequence = sequence;
            Timestamp = timestamp;
            Payload = payload;
            Missed = missed;
        }
    }
}