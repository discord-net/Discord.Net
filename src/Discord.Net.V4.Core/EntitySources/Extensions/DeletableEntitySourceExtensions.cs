namespace Discord.Extensions;

public static class DeletableEntitySourceExtensions
{
    public static Task DeleteAsync<TLoadable, TId, TEntity>(
        this TLoadable entity,
        RequestOptions? options = null,
        CancellationToken token = default
    )
        where TLoadable : ILoadableEntity<TId, TEntity>, IPathable, IClientProvider
        where TEntity : class, IPathableDeletable<TEntity, TId>, IEntity<TId>
        where TId : IEquatable<TId>
        => IPathableDeletable<TEntity, TId>.DeleteAsync(entity.Client, entity, entity.Id, options, token);
}
