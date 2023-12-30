using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

internal sealed class RateLimiter
{
    private sealed class RequestHandle(Action onComplete) : IDisposable
    {
        private bool _isComplete;

        public void Dispose()
        {
            lock (this)
            {
                if (_isComplete)
                    return;

                _isComplete = true;
                onComplete();
            }
        }
    }

    [MemberNotNullWhen(true, nameof(_globalReset))]
    public bool IsGlobalLimit
        => _globalReset is not null && _globalReset >= DateTimeOffset.UtcNow;

    private readonly List<Bucket> _buckets = [];
    private readonly SemaphoreSlim _bucketSemaphore = new(1, 1);
    private readonly SemaphoreSlim _requestQueue = new SemaphoreSlim(1,1);

    private readonly List<RatelimitContract> _contracts = [];

    private DateTimeOffset? _globalReset;

    public void TriggerGlobalLimit(DateTimeOffset reset)
    {
        _globalReset = reset;
    }

    public async Task<IDisposable> GetRequestHandleAsync(CancellationToken token = default)
    {
        await _requestQueue.WaitAsync(token);

        // are we global'd
        if (IsGlobalLimit)
            await Task.Delay(_globalReset.Value - DateTimeOffset.UtcNow, token);

        return new RequestHandle(() => _requestQueue.Release());
    }

    public async Task<RatelimitContract> AcquireContractAsync(ApiRoute route, CancellationToken token = default)
    {
        return await CreateContractForBucketAsync(await GetOrCreateBucketAsync(route, token), token);
    }
    private async ValueTask<RatelimitContract> CreateContractForBucketAsync(Bucket bucket, CancellationToken token = default)
    {
        await bucket.AcquireHandleAsync(token);
        var contract = new RatelimitContract(this, bucket, ratelimitContract => _contracts.Remove(ratelimitContract));
        _contracts.Add(contract);
        return contract;
    }

    private async Task<Bucket> GetOrCreateBucketAsync(ApiRoute route, CancellationToken token = default)
    {
        await _bucketSemaphore.WaitAsync(token);

        try
        {
            var bucket = _buckets.FirstOrDefault(x => x.Id.Represents(route));

            if (bucket is not null)
                return bucket;

            bucket = new Bucket(new BucketId(route.Endpoint, Info: route.Bucket));

            _buckets.Add(bucket);

            return bucket;
        }
        finally
        {
            _bucketSemaphore.Release();
        }
    }
}
