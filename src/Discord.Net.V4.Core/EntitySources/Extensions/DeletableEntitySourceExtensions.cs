namespace Discord.Extensions;

public static class DeletableEntitySourceExtensions
{
    public static Task DeleteAsync<TSource, TId, TEntity>(
        this TSource entity,
        RequestOptions? options = null,
        CancellationToken token = default
    )
        where TSource : IEntitySource<TId, TEntity>
        where TEntity : class, IDeletable<TId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
        => IDeletable<TId, TEntity>.DeleteAsync(entity.Client, entity, entity.Id, options, token);
}
