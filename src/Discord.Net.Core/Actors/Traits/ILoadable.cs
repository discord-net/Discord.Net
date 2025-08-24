namespace Discord;

public interface ILoadable<TResult>
{
    ValueTask<TResult> GetAsync(RequestOptions options = default);
}