using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IEnumerable<T>> Flatten<T>(this IAsyncEnumerable<IReadOnlyCollection<T>> source)
        {
            return (await source.ToArray().ConfigureAwait(false)).SelectMany(x => x);
        }
    }
}
