using System.Diagnostics.CodeAnalysis;

namespace Discord;

public sealed class Bucket
{
    public bool Empty => _limit is null;
    internal BucketId Id { get; }

    private uint? _remaining;
    private uint? _limit;

    private uint _window;

    private bool _lent;

    private DateTimeOffset? _reset;

    private readonly TaskCompletionSource _initializationSource;
    private readonly object _lockObj = new();

    internal Bucket(BucketId id)
    {
        Id = id;
        _initializationSource = new TaskCompletionSource();
    }

    public void Update(in RatelimitInfo info)
    {
        lock (_lockObj)
        {
            _remaining = info.Remaining;
            _limit ??= info.Limit;
            _reset = info.Reset;

            if (!_initializationSource.Task.IsCompleted)
                _initializationSource.TrySetResult();
        }
    }

    public async ValueTask AcquireHandleAsync(CancellationToken token = default)
    {
        // check if we can lend a handle before, this is for the first request only.
        lock (_lockObj)
        {
            if (_remaining is null && !_lent)
            {
                _lent = true;
                return;
            }
        }

        // if the first request has been lent but not processed, await the initialization source.
        if (_lent && _remaining is null)
            await _initializationSource.Task;

        // our standard queue for requests, we delegate them out until the limit has been reached, this acts
        // much like a semaphore, but instead of blocking we just proceed to the delay.
    in_limits:
        lock (_lockObj)
        {
            if (_window < _limit)
            {
                _window++;
                return;
            }
        }

        // wait for the reset if theres no more handles remaining
        await Task.Delay(
            (_reset ??throw new NullReferenceException("This is not allowed to be null")) - DateTimeOffset.Now,
            token
        );

        // many tasks can wait here, so we only allow one to reset the limit, the rest go to the 'in_limits' statement
        lock (_lockObj)
        {
            if (_window == _limit)
            {
                _window = 0;
                return;
            }
        }

        // only reached when the queue is re-initialized by the above statement, we can go back to standard
        // handle-queueing
        goto in_limits;
    }
}
