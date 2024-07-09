using Discord.Models;

namespace Discord;

public static class IdentifiableExtensions
{
    public static IIdentifiableEntityOrModel<TId, TDescEntity, TDescModel> OfDescendingIdentity<TId, TEntity, TModel, TDescEntity, TDescModel>(
        this IIdentifiableEntityOrModel<TId, TEntity, TModel> identity
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TDescEntity : class, IEntity<TId>, IEntityOf<TDescModel>
        where TDescModel : class, TModel, IEntityModel<TId>
    {
        if (identity.Entity is null) return IIdentifiableEntityOrModel<TId, TDescEntity, TDescModel>.Of(identity.Id);

        if(identity.Entity is not TDescEntity desc)
            throw new InvalidCastException($"Cannot cast {typeof(TEntity)} to {typeof(TDescEntity)}");

        return IIdentifiableEntityOrModel<TId, TDescEntity, TDescModel>.Of(desc);
    }
}
