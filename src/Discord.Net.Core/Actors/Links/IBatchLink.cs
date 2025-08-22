namespace Discord;

public interface IBatchLink<TEntity>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(RequestOptions options = default);
}