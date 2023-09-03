namespace Discord;

public interface IDeletable
{
    ValueTask DeleteAsync(RequestOptions? options = null, CancellationToken token = default);
}
