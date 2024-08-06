using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

public class GatewayIndexableActor<TActor, TId, TEntity>(Func<TId, TActor> factory) :
    IIndexableActor<TActor, TId, TEntity>
    where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId>>
    where TId : IEquatable<TId>
    where TEntity : GatewayEntity<TId>, IEntityOf<IEntityModel<TId>>
{
    private readonly WeakDictionary<TId, TActor> _cache = new();

    public TActor this[TId id] => Specifically(id);

    public TActor Specifically(TId id)
        => _cache.GetOrAdd(id, factory);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    internal TActor? FirstOrDefault(TActor? defaultValue = null)
        => _cache.FirstOrDefault(defaultValue);

    public static TActor operator >>(
        GatewayIndexableActor<TActor, TId, TEntity> source,
        IIdentifiable<TId, TEntity, TActor, IEntityModel<TId>> identity
    ) => identity.Actor ?? source[identity.Id];
}
