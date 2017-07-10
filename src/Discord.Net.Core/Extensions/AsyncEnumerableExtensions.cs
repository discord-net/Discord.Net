using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            return new PagedCollectionEnumerator<T>(source);
        }
        
        internal class PagedCollectionEnumerator<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
        {
            readonly IAsyncEnumerator<IEnumerable<T>> _source;
            IEnumerator<T> _enumerator;

            public IAsyncEnumerator<T> GetEnumerator() => this;

            internal PagedCollectionEnumerator(IAsyncEnumerable<IEnumerable<T>> source)
            {
                _source = source.GetEnumerator();
            }

            public T Current => _enumerator.Current;

            public void Dispose()
            {
                _enumerator?.Dispose();
                _source.Dispose();
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if(!_enumerator?.MoveNext() ?? true)
                {
                    if (!await _source.MoveNext(cancellationToken).ConfigureAwait(false))
                        return false;

                    _enumerator?.Dispose();
                    _enumerator = _source.Current.GetEnumerator();
                    return _enumerator.MoveNext();
                }

                return true;
            }
        }
    }
}
