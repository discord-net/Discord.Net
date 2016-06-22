namespace Discord.Net.Queue
{
    public sealed class Bucket
    {
        /// <summary> Gets the unique identifier for this bucket. </summary>
        public string Id { get; }
        /// <summary> Gets the name of this bucket. </summary>
        public string Name { get; }
        /// <summary> Gets the amount of requests that may be sent per window. </summary>
        public int WindowCount { get; }
        /// <summary> Gets the length of this bucket's window, in seconds. </summary>
        public int WindowSeconds { get; }
        /// <summary> Gets the type of account this bucket affects. </summary>
        public BucketTarget Target { get; }
        /// <summary> Gets this bucket's parent. </summary>
        public GlobalBucket? Parent { get; }

        internal Bucket(string id, int windowCount, int windowSeconds, BucketTarget target, GlobalBucket? parent = null)
            : this(id, id, windowCount, windowSeconds, target, parent) { }
        internal Bucket(string id, string name, int windowCount, int windowSeconds, BucketTarget target, GlobalBucket? parent = null)
        {
            Id = id;
            Name = name;
            WindowCount = windowCount;
            WindowSeconds = windowSeconds;
            Target = target;
            Parent = parent;
        }
    }
}
