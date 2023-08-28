using System.Collections.Immutable;

namespace Discord;

internal sealed class AsyncEvent<T>
    where T : Delegate
{
    public bool HasSubscribers
        => _subscriptions.Count > 0;

    public IReadOnlyCollection<T> Subscribers
        => _subscriptions.ToImmutableArray();

    private readonly HashSet<T> _subscriptions;

    public AsyncEvent()
    {
        _subscriptions = new();
    }

    public bool Add(T handler)
        => _subscriptions.Add(handler);

    public bool Remove(T handler)
        => _subscriptions.Remove(handler);
}
