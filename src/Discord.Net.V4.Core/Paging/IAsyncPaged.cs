using System.Collections.Immutable;

namespace Discord.Paging;

public interface IAsyncPaged<out T> : IAsyncEnumerable<IReadOnlyCollection<T>>
{
    int? PageSize { get; }
}
