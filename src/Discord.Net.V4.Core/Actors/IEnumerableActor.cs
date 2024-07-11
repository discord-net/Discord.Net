namespace Discord;

public interface IEnumerableActor<in TId, TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
}
