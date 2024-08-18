using Discord.Models;

namespace Discord.Rest.Actors;

public class RestIndexableLink<TActor, TId, TEntity>(
    Func<TId, TActor> actorFactory
) :
    IIndexableLink<TActor, TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    public TActor this[TId id] => Specifically(id);

    // todo: cache the result of actor factory
    public TActor Specifically(TId id)
        => actorFactory(id);
}
