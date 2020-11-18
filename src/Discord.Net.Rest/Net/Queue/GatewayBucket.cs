using System.Collections.Immutable;

namespace Discord.Net.Queue
{
    public enum GatewayBucketType
    {
        Unbucketed = 0,
        Identify = 1,
        PresenceUpdate = 2,
    }
    internal struct GatewayBucket
    {
        private static readonly ImmutableDictionary<GatewayBucketType, GatewayBucket> DefsByType;
        private static readonly ImmutableDictionary<BucketId, GatewayBucket> DefsById;

        static GatewayBucket()
        {
            var buckets = new[]
            {
                // Limit is 120/60s, but 3 will be reserved for heartbeats (2 for possible heartbeats in the same timeframe and a possible failure)
                new GatewayBucket(GatewayBucketType.Unbucketed, BucketId.Create(null, "<gateway-unbucketed>", null), 117, 60),
                new GatewayBucket(GatewayBucketType.Identify, BucketId.Create(null, "<gateway-identify>", null), 1, 5),
                new GatewayBucket(GatewayBucketType.PresenceUpdate, BucketId.Create(null, "<gateway-presenceupdate>", null), 5, 60),
            };

            var builder = ImmutableDictionary.CreateBuilder<GatewayBucketType, GatewayBucket>();
            foreach (var bucket in buckets)
                builder.Add(bucket.Type, bucket);
            DefsByType = builder.ToImmutable();

            var builder2 = ImmutableDictionary.CreateBuilder<BucketId, GatewayBucket>();
            foreach (var bucket in buckets)
                builder2.Add(bucket.Id, bucket);
            DefsById = builder2.ToImmutable();
        }

        public static GatewayBucket Get(GatewayBucketType type) => DefsByType[type];
        public static GatewayBucket Get(BucketId id) => DefsById[id];

        public GatewayBucketType Type { get; }
        public BucketId Id { get; }
        public int WindowCount { get; set; }
        public int WindowSeconds { get; set; }

        public GatewayBucket(GatewayBucketType type, BucketId id, int count, int seconds)
        {
            Type = type;
            Id = id;
            WindowCount = count;
            WindowSeconds = seconds;
        }
    }
}
