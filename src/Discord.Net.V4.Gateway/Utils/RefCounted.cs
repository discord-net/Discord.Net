namespace Discord.Gateway;

internal sealed class RefCounted<T> : IRefCounted<T> where T : class
{
    public bool HasValue
        => _value is not null || _weak.TryGetTarget(out _);

    public T Value => EnsureValue();

    private T? _value;
    private int _window;
    private readonly WeakReference<T> _weak;
    private readonly object _syncRoot = new();

    public RefCounted(T value)
    {
        _value = value;
        _window = 1;
        _weak = new WeakReference<T>(value);
    }

    public void AddReference()
        => _window++;

    public void Dispose()
    {
        if (_window <= 0 || Interlocked.Decrement(ref _window) <= 0)
            _value = null!;
    }

    private T EnsureValue()
    {
        if (_value is not null)
            return _value;

        // if window is 0 here, we're resurrecting
        if (_window <= 0)
        {
            _window = 1;
        }

        lock (_syncRoot)
        {
            if (_value is not null)
                return _value;

            if (_weak.TryGetTarget(out _value))
                return _value;

            _window = -1;
            throw new InvalidOperationException("Ref counted object no longer exists.");
        }
    }
}

internal interface IRefCounted<out T> : IDisposable where T : class
{
    T Value { get; }
    bool HasValue { get; }
}
