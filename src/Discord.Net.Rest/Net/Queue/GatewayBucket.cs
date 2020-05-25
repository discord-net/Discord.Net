using Discord.Rest;
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
        private static ImmutableDictionary<GatewayBucketType, GatewayBucket> DefsByType;
        private static ImmutableDictionary<string, GatewayBucket> DefsById;
        private static string IdentifySemaphoreName;

        static GatewayBucket()
        {
            SetLimits(GatewayLimits.GetOrCreate(null));
        }

        public static GatewayBucket Get(GatewayBucketType type) => DefsByType[type];
        public static GatewayBucket Get(string id) => DefsById[id];
        public static string GetIdentifySemaphoreName() => IdentifySemaphoreName;

        public static void SetLimits(GatewayLimits limits)
        {
            limits = GatewayLimits.GetOrCreate(limits);
            Preconditions.GreaterThan(limits.Global.Count, 0, nameof(limits.Global.Count), "Global count must be greater than zero.");
            Preconditions.GreaterThan(limits.Global.Seconds, 0, nameof(limits.Global.Seconds), "Global seconds must be greater than zero.");
            Preconditions.GreaterThan(limits.Identify.Count, 0, nameof(limits.Identify.Count), "Identify count must be greater than zero.");
            Preconditions.GreaterThan(limits.Identify.Seconds, 0, nameof(limits.Identify.Seconds), "Identify seconds must be greater than zero.");
            Preconditions.GreaterThan(limits.PresenceUpdate.Count, 0, nameof(limits.PresenceUpdate.Count), "PresenceUpdate count must be greater than zero.");
            Preconditions.GreaterThan(limits.PresenceUpdate.Seconds, 0, nameof(limits.PresenceUpdate.Seconds), "PresenceUpdate seconds must be greater than zero.");

            var buckets = new[]
            {
                new GatewayBucket(GatewayBucketType.Unbucketed, "<gateway-unbucketed>", limits.Global.Count, limits.Global.Seconds),
                new GatewayBucket(GatewayBucketType.Identify, "<gateway-identify>", limits.Identify.Count, limits.Identify.Seconds),
                new GatewayBucket(GatewayBucketType.PresenceUpdate, "<gateway-presenceupdate>", limits.Identify.Count, limits.Identify.Seconds),
            };

            var builder = ImmutableDictionary.CreateBuilder<GatewayBucketType, GatewayBucket>();
            foreach (var bucket in buckets)
                builder.Add(bucket.Type, bucket);
            DefsByType = builder.ToImmutable();

            var builder2 = ImmutableDictionary.CreateBuilder<string, GatewayBucket>();
            foreach (var bucket in buckets)
                builder2.Add(bucket.Id, bucket);
            DefsById = builder2.ToImmutable();

            IdentifySemaphoreName = limits.IdentifySemaphoreName;
        }

        public GatewayBucketType Type { get; }
        public string Id { get; }
        public int WindowCount { get; set; }
        public int WindowSeconds { get; set; }

        public GatewayBucket(GatewayBucketType type, string id, int count, int seconds)
        {
            Type = type;
            Id = id;
            WindowCount = count;
            WindowSeconds = seconds;
        }
    }
}
