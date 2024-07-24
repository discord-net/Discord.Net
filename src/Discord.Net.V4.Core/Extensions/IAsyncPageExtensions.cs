using Discord.Paging;
using System.Collections.Immutable;

namespace Discord;

public static class AsyncPageExtensions
{
    public static async Task<IReadOnlyCollection<T>> FlattenAsync<T>(this IAsyncPaged<T> asyncPaged)
    {
        var items = new List<T>();

        await foreach (var page in asyncPaged)
            items.AddRange(page);

        return items.ToImmutableArray();
    }
}
