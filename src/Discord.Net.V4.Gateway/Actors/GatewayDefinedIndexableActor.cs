using Discord.Models;

namespace Discord.Gateway;

public sealed class GatewayDefinedIndexableLink<TActor, TId, TEntity, TCore>(
    IReadOnlyCollection<TId> ids,
    GatewayIndexableLink<TActor, TId, TEntity> indexableLink
) :
    IDefinedIndexableLink<TActor, TId, TCore>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>, IActor<TId, TCore>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>, IEntityOf<IEntityModel<TId>>
    where TCore : class, IEntity<TId, IEntityModel<TId>>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public TActor Specifically(TId id) => indexableLink.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(indexableLink.Specifically);
}
