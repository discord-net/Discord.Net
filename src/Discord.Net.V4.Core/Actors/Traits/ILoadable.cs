namespace Discord;

public interface ILoadable<TEntity>
{
    ValueTask<TEntity> GetAsync(RequestOptions options = default);
}