using System.Collections.Concurrent;
using Discord.Rest.Api;

namespace Discord.Rest.Ratelimits;

public sealed class RateLimiter
{
    private readonly Dictionary<BucketId, Bucket> _buckets = [];
    
    public Bucket GetOrCreateBucket<T>(T route) where T : IRoute
    {
        var id = BucketId.FromRoute(route);

        if (!_buckets.TryGetValue(id, out var bucket))
            bucket = _buckets[id] = new();

        return bucket;
    }
}