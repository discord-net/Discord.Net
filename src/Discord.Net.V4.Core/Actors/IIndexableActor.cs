using Discord.Models;

namespace Discord;

public interface IIndexableActor<out TActor, in TId, out TEntity>
    where TActor : class, IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>, IEntityOf<IEntityModel<TId>>
    where TId : IEquatable<TId>
{
    TActor this[TId id] => Specifically(id);

    TActor Specifically(TId id);

    public static TActor operator >>(
        IIndexableActor<TActor, TId, TEntity> source,
        IIdentifiable<TId, TEntity, TActor, IEntityModel<TId>> identity
    ) => identity.Actor ?? source[identity.Id];
}
