namespace Discord.Net.Queue
{
    internal struct BucketDefinition
    {
        public int WindowCount { get; }
        public int WindowSeconds { get; }
        public GlobalBucket? Parent { get; }

        public BucketDefinition(int windowCount, int windowSeconds, GlobalBucket? parent = null)
        {
            WindowCount = windowCount;
            WindowSeconds = windowSeconds;
            Parent = parent;
        }
    }
}
