namespace Discord;

public interface IModifyable<T>
{
    Task ModifyAsync(Action<T> func, RequestOptions? options = null, CancellationToken token = default);
}
