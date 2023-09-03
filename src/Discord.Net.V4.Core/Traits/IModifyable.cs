namespace Discord;

public interface IModifyable<T>
{
    ValueTask ModifyAsync(Action<T> func, RequestOptions? options = null, CancellationToken token = default);
}
