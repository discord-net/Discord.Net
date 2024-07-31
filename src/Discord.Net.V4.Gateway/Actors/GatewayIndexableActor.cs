namespace Discord.Gateway;

public sealed class GatewayIndexableActor<TActor, TId, TEntity>(Func<TId, TActor> factory) :
    IIndexableActor<TActor, TId, TEntity>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>
{
    private readonly WeakDictionary<TId, TActor> _cache = new();

    public TActor this[TId id] => Specifically(id);

    public TActor Specifically(TId id)
        => _cache.GetOrAdd(id, factory);
}
