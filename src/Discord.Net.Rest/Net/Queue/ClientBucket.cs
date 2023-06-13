using System.Collections.Immutable;

namespace Discord.Net.Queue
{
    public enum ClientBucketType
    {
        Unbucketed = 0,
        SendEdit = 1
    }
    internal struct ClientBucket
    {
        private static readonly ImmutableDictionary<ClientBucketType, ClientBucket> DefsByType;
        private static readonly ImmutableDictionary<BucketId, ClientBucket> DefsById;

        static ClientBucket()
        {
            var buckets = new[]
            {
                new ClientBucket(ClientBucketType.Unbucketed, BucketId.Create(null, "<unbucketed>", null), 10, 10),
                new ClientBucket(ClientBucketType.SendEdit, BucketId.Create(null, "<send_edit>", null), 10, 10)
            };

            var builder = ImmutableDictionary.CreateBuilder<ClientBucketType, ClientBucket>();
            foreach (var bucket in buckets)
                builder.Add(bucket.Type, bucket);
            DefsByType = builder.ToImmutable();

            var builder2 = ImmutableDictionary.CreateBuilder<BucketId, ClientBucket>();
            foreach (var bucket in buckets)
                builder2.Add(bucket.Id, bucket);
            DefsById = builder2.ToImmutable();
        }

        public static ClientBucket Get(ClientBucketType type) => DefsByType[type];
        public static ClientBucket Get(BucketId id) => DefsById[id];

        public ClientBucketType Type { get; }
        public BucketId Id { get; }
        public int WindowCount { get; }
        public int WindowSeconds { get; }

        public ClientBucket(ClientBucketType type, BucketId id, int count, int seconds)
        {
            Type = type;
            Id = id;
            WindowCount = count;
            WindowSeconds = seconds;
        }
    }
}
