using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Flattens the specified pages into one <see cref="IEnumerable{T}"/> asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> FlattenAsync<T>(this IAsyncEnumerable<IEnumerable<T>> source)
        {
            return await source.Flatten().ToArray().ConfigureAwait(false);
        }

        public static IAsyncEnumerable<T> Flatten<T>(this IAsyncEnumerable<IEnumerable<T>> source)
        {
            return source.SelectMany(enumerable => enumerable.ToAsyncEnumerable());
        }
    }
}
