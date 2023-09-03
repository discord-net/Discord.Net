namespace Discord;

public interface IDeletable
{
    Task DeleteAsync(RequestOptions? options = null, CancellationToken token = default);
}
