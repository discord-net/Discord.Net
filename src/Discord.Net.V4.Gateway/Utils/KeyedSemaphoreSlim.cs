namespace Discord.Gateway;

internal sealed class KeyedSemaphoreSlim<TKey>(int initial, int maximum)
    where TKey : notnull
{
    private sealed class KeyedSemaphoreScope(TKey key, KeyedSemaphoreSlim<TKey> owner, SemaphoreSlim semaphore) : IDisposable
    {
        public SemaphoreSlim Semaphore = semaphore;
        public int Window = 1;

        public void Dispose()
        {
            if (Interlocked.Decrement(ref Window) > 0) return;

            owner.OnScopeRelease(key);
            Semaphore.Dispose();
            Semaphore = null!;
        }
    }

    private readonly Dictionary<TKey, KeyedSemaphoreScope> _semaphores = [];
    private readonly object _syncRoot = new();

    private readonly Func<SemaphoreSlim> _factory = () => new(initial, maximum);

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

            entry = _semaphores[key] = new(key, this, _factory());

            semaphoreSlim = entry.Semaphore;
            return entry;
        }
    }

    private void OnScopeRelease(TKey key)
    {
        lock (_syncRoot)
        {
            _semaphores.Remove(key);
        }
    }
}
