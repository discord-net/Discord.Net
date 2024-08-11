using Discord.Models;

namespace Discord.Gateway;

public sealed class GatewayDefinedIndexableActor<TActor, TId, TEntity, TCore>(
    IReadOnlyCollection<TId> ids,
    GatewayIndexableActor<TActor, TId, TEntity> indexableActor
) :
    IDefinedIndexableActor<TActor, TId, TCore>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>, IActor<TId, TCore>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>, IEntityOf<IEntityModel<TId>>
    where TCore : class, IEntity<TId, IEntityModel<TId>>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public TActor Specifically(TId id) => indexableActor.Specifically(id);

    public IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(indexableActor.Specifically);
}
