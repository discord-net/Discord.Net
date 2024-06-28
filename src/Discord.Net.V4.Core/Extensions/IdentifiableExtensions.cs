using Discord.Models;

namespace Discord;

public static class IdentifiableExtensions
{
    public static IIdentifiableEntityOrModel<TId, TEntity> Identity<TId, TEntity, TModel>(this TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        where TModel : IEntityModel<TId>
        => new IdentifiableEntityOrModel<TId, TEntity, TModel>(entity);

    public static IdentifiableEntityOrModel<TId, TEntity, TModel>? Identity<TId, TEntity, TModel>(this TModel? model,
        Func<TModel, TEntity> factory)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>
        where TModel : IEntityModel<TId>
        => model is not null ? new(model, factory) : null;
}
