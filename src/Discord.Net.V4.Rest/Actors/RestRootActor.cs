namespace Discord.Rest.Actors;

public class RestRootActor<TActor, TId, TEntity>(Func<TId, TActor> actorFactory) : IRootActor<TActor, TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    public TActor Specifically(TId id)
        => actorFactory(id);
}
