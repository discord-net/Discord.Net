using Discord.Models;

namespace Discord.Rest.Actors;

public sealed class RestDefinedIndexableActor<TActor, TId, TEntity>(
    IReadOnlyCollection<TId> ids,
    RestIndexableActor<TActor, TId, TEntity> indexableActor
) :
    IDefinedIndexableActor<TActor, TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public RestDefinedIndexableActor(IReadOnlyCollection<TId> ids, Func<TId, TActor> actorFactory)
        : this(ids, new RestIndexableActor<TActor, TId, TEntity>(actorFactory))
    {
    }

    public TActor Specifically(TId id) => indexableActor.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(indexableActor.Specifically);
}
