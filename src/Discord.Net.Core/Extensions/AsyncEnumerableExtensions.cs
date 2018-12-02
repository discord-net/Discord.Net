using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary> An extension class for squashing <see cref="IAsyncEnumerable{T}"/>. </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary> Flattens the specified pages into one <see cref="IEnumerable{T}"/> asynchronously. </summary>
        public static async Task<IEnumerable<T>> FlattenAsync<T>(this IAsyncEnumerable<IEnumerable<T>> source)
        {
            return await source.Flatten().ToArray().ConfigureAwait(false);
        }
        /// <summary> Flattens the specified pages into one <see cref="IAsyncEnumerable{T}"/>. </summary>
        public static IAsyncEnumerable<T> Flatten<T>(this IAsyncEnumerable<IEnumerable<T>> source)
        {
            return source.SelectMany(enumerable => enumerable.ToAsyncEnumerable());
        }
    }
}
