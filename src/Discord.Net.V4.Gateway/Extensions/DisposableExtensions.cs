namespace Discord.Gateway;

internal static class DisposableExtensions
{
    private sealed class CombinedDisposable(params IDisposable[] disposables) : IDisposable
    {
        private IDisposable[]? _disposables = disposables;

        public void Dispose()
        {
            var disposables = Interlocked.Exchange(ref _disposables, null);

            if (disposables is null) return;

            foreach (var disposable in disposables)
                disposable.Dispose();
        }
    }

    public static IDisposable Combine(this IDisposable source, params IDisposable[] others)
        => new CombinedDisposable([source, ..others]);
}