using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    internal class PagedAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public int PageSize { get; }

        private readonly ulong? _start;
        private readonly int? _count;
        private readonly Func<PageInfo, CancellationToken, Task<IReadOnlyCollection<T>>> _getPage;
        private readonly Func<PageInfo, IReadOnlyCollection<T>, bool> _nextPage;

        public PagedAsyncEnumerable(int pageSize, Func<PageInfo, CancellationToken, Task<IReadOnlyCollection<T>>> getPage, Func<PageInfo, IReadOnlyCollection<T>, bool> nextPage = null,
            ulong? start = null, int? count = null)
        {
            PageSize = pageSize;
            _start = start;
            _count = count;

            _getPage = getPage;
            _nextPage = nextPage;
        }

        public IAsyncEnumerator<T> GetEnumerator() => new Enumerator(this);
        internal class Enumerator : IAsyncEnumerator<T>
        {
            private readonly PagedAsyncEnumerable<T> _source;
            private readonly PageInfo _info;
            private IReadOnlyCollection<T> _currentPage;
            private IEnumerator<T> _currentPageEnumerator;

            public T Current => _currentPageEnumerator.Current;

            public Enumerator(PagedAsyncEnumerable<T> source)
            {
                _source = source;
                _info = new PageInfo(source._start, source._count, source.PageSize);
            }

            public async Task<bool> MoveNext(CancellationToken cancelToken)
            {
                if (_info.Remaining == 0)
                    return false;

                if (_currentPage == null || _currentPageEnumerator == null || !_currentPageEnumerator.MoveNext())
                {
                    _currentPageEnumerator?.Dispose();

                    var data = await _source._getPage(_info, cancelToken).ConfigureAwait(false);
                    _currentPage = new Page<T>(_info, data);
                    _currentPageEnumerator = _currentPage.GetEnumerator();

                    _info.Page++;
                    if (_info.Remaining != null)
                    {
                        if (_currentPage.Count >= _info.Remaining)
                            _info.Remaining = 0;
                        else
                            _info.Remaining -= _currentPage.Count;
                    }
                    else
                    {
                        if (_currentPage.Count == 0)
                            _info.Remaining = 0;
                    }
                    _info.PageSize = _info.Remaining != null ? Math.Min(_info.Remaining.Value, _source.PageSize) : _source.PageSize;

                    if (_info.Remaining != 0)
                    {
                        if (!_source._nextPage(_info, data))
                            _info.Remaining = 0;
                    }
                }

                return true;
            }
            
            public void Dispose()
            {
                _currentPageEnumerator.Dispose();
            }
        }
    }
}