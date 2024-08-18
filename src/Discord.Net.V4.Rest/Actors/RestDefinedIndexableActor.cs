using Discord.Models;

namespace Discord.Rest.Actors;

public sealed class RestDefinedIndexableLink<TActor, TId, TEntity>(
    IReadOnlyCollection<TId> ids,
    RestIndexableLink<TActor, TId, TEntity> indexableLink
) :
    IDefinedIndexableLink<TActor, TId, TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : RestEntity<TId>, IEntity<TId, IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public RestDefinedIndexableLink(IReadOnlyCollection<TId> ids, Func<TId, TActor> actorFactory)
        : this(ids, new RestIndexableLink<TActor, TId, TEntity>(actorFactory))
    {
    }

    public TActor Specifically(TId id) => indexableLink.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(indexableLink.Specifically);
}
