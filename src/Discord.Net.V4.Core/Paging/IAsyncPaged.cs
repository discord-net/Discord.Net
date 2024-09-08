namespace Discord;

public interface IAsyncPaged<out T> : IAsyncEnumerable<T>
{
    int? PageSize { get; }
}
