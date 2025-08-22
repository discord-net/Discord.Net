namespace Discord.Models;

public interface ILoadable<TEntity>
{
    ValueTask<TEntity> GetAsync(RequestOptions options = default);
}