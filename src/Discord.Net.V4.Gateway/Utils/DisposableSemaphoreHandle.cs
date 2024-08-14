namespace Discord.Gateway;

internal sealed class DisposableSemaphoreHandle : IDisposable
{
    private SemaphoreSlim? _semaphore;
    
    private DisposableSemaphoreHandle(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public static async ValueTask<DisposableSemaphoreHandle> CreateAsync(SemaphoreSlim semaphoreSlim, CancellationToken token = default)
    {
        await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);

        return new(semaphoreSlim);
    }

    public void Dispose()
        => Interlocked.Exchange(ref _semaphore, null)?.Release();
}