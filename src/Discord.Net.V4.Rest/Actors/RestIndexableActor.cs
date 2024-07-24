namespace Discord.Rest.Actors;

public class RestIndexableActor<TActor, TId, TEntity>(Func<TId, TActor> actorFactory) : IIndexableActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : RestEntity<TId>
    where TId : IEquatable<TId>
{
    public TActor this[TId id] => Specifically(id);
    // todo: cache the result of actor factory
    public TActor Specifically(TId id)
        => actorFactory(id);
}
