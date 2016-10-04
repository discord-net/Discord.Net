using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    internal class PagedAsyncEnumerable<T> : IAsyncEnumerable<IReadOnlyCollection<T>>
    {
        public int PageSize { get; }

        private readonly ulong? _start;
        private readonly uint? _count;
        private readonly Func<PageInfo, CancellationToken, Task<IReadOnlyCollection<T>>> _getPage;
        private readonly Action<PageInfo, IReadOnlyCollection<T>> _nextPage;

        public PagedAsyncEnumerable(int pageSize, Func<PageInfo, CancellationToken, Task<IReadOnlyCollection<T>>> getPage, Action<PageInfo, IReadOnlyCollection<T>> nextPage = null,
            ulong? start = null, uint? count = null)
        {
            PageSize = pageSize;
            _start = start;
            _count = count;

            _getPage = getPage;
            _nextPage = nextPage;
        }

        public IAsyncEnumerator<IReadOnlyCollection<T>> GetEnumerator() => new Enumerator(this);
        internal class Enumerator : IAsyncEnumerator<IReadOnlyCollection<T>>
        {
            private readonly PagedAsyncEnumerable<T> _source;
            private readonly PageInfo _info;

            public IReadOnlyCollection<T> Current { get; private set; }

            public Enumerator(PagedAsyncEnumerable<T> source)
            {
                _source = source;
                _info = new PageInfo(source._start, source._count, source.PageSize);
            }

            public async Task<bool> MoveNext(CancellationToken cancelToken)
            {
                var data = await _source._getPage(_info, cancelToken);
                Current = new Page<T>(_info, data);

                _info.Page++;
                _info.Remaining -= (uint)Current.Count;
                _info.PageSize = _info.Remaining != null ? (int)Math.Min(_info.Remaining.Value, (ulong)_source.PageSize) : _source.PageSize;
                _source?._nextPage(_info, data);

                return _info.Remaining > 0;
            }
            
            public void Dispose() { Current = null; }
        }
    }
}