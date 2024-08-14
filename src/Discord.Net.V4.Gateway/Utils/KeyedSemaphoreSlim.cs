using JetBrains.Annotations;

namespace Discord.Gateway;

internal sealed class KeyedSemaphoreSlim<TKey>(int initial, int maximum)
    where TKey : notnull
{
    private sealed class KeyedSemaphoreScope(TKey key, KeyedSemaphoreSlim<TKey> owner, SemaphoreSlim semaphore) : IDisposable
    {
        public TKey Key { get; } = key;
        public readonly SemaphoreSlim Semaphore = semaphore;
        private KeyedSemaphoreSlim<TKey>? _owner = owner;
        public int Window = 1;

        public void Dispose()
        {
            if (_owner is null) return;
            
            if (Interlocked.Decrement(ref Window) > 0) return;
            
            Interlocked.Exchange(ref _owner, null)?.OnScopeRelease(this);
        }
    }

    private readonly Queue<SemaphoreSlim> _pool = new();
    private readonly Dictionary<TKey, KeyedSemaphoreScope> _semaphores = [];
    private readonly object _syncRoot = new();

    private readonly Func<SemaphoreSlim> _factory = () => new(initial, maximum);

    [MustDisposeResource]
    public IDisposable Get(TKey key, out SemaphoreSlim semaphoreSlim)
    {
        lock (_syncRoot)
        {
            if (_semaphores.TryGetValue(key, out var entry))
            {
                Interlocked.Increment(ref entry.Window);
                semaphoreSlim = entry.Semaphore;
                return entry;
            }

            if (!_pool.TryDequeue(out semaphoreSlim!))
                semaphoreSlim = _factory();
            
            return _semaphores[key] = new(key, this, semaphoreSlim);
        }
    }

    private void OnScopeRelease(KeyedSemaphoreScope scope)
    {
        lock (_syncRoot)
        {
            if(_semaphores.Remove(scope.Key))
                _pool.Enqueue(scope.Semaphore);
        }
    }
}
