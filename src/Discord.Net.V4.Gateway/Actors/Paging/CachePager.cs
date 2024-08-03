using Discord.Gateway.State;
using Discord.Models;
using Discord.Paging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.Paging;

internal sealed class CachePager<TId, TEntity, TModel, TParams> : IAsyncPaged<TEntity>
    where TId : IEquatable<TId>
    where TEntity :
    class,
    IStoreInfoProvider<TId, TModel>,
    IBrokerProvider<TId, TEntity, TModel>,
    ICacheableEntity<TEntity, TId, TModel>,
    IContextConstructable<TEntity, TModel, ICacheConstructionContext, DiscordGatewayClient>
    where TModel : class, IEntityModel<TId>
    where TParams : IPagingParams
{
    public int? PageSize => _pageSize;

    private readonly DiscordGatewayClient _client;
    private readonly CachePathable _path;

    private readonly Optional<TId> _upperBounds;
    private readonly Optional<TId> _lowerBounds;
    private readonly Direction? _direction;
    private readonly IComparer<TId>? _idComparer;

    private readonly int _pageSize;
    private readonly int? _total;

    private IEntityBroker<TId, TEntity, TModel>? _broker;
    private IStoreInfo<TId, TModel>? _storeInfo;

    public CachePager(
        DiscordGatewayClient client,
        CachePathable path,
        TParams? pageParams)
    {
        _client = client;
        _path = path;

        if (pageParams is IDirectionalPagingParams<TId> directional)
        {
            if (directional is IBetweenPagingParams<TId> {IsBetween: true} between)
                _upperBounds = between.Before;

            _lowerBounds = directional.From;
            _direction = directional.Direction;

            _idComparer = Comparer<TId>.Default;
        }

        _pageSize = pageParams?.PageSize is not null
            ? Math.Min(pageParams.PageSize.Value, TParams.MaxPageSize)
            : TParams.MaxPageSize;

        _total = pageParams?.Total;
    }

    public IAsyncEnumerator<IReadOnlyCollection<TEntity>> GetAsyncEnumerator(CancellationToken token = default)
        => new Enumerator(this, token);

    private struct Enumerator : IAsyncEnumerator<IReadOnlyCollection<TEntity>>
    {
        // TODO: immutability
        public IReadOnlyCollection<TEntity> Current { get; private set; }

        private bool _hasInit;

        private List<IAsyncEnumerator<TModel>> _enumerators;

        private TId? _lastId;
        private List<TModel>? _view;
        private int _oldestViewIndex;

        private TModel[] _page;
        private int _storeCount;

        private bool _enumerationEnded;

        private readonly CachePager<TId, TEntity, TModel, TParams> _pager;
        private readonly CancellationToken _token;

        private readonly bool _isQuery;

        public Enumerator(
            CachePager<TId, TEntity, TModel, TParams> pager,
            CancellationToken token)
        {
            _enumerators = [];

            _pager = pager;
            _token = token;

            _isQuery = _pager._idComparer is not null && _pager._lowerBounds.IsSpecified;

            _page = new TModel[_pager._pageSize];

            Current = [];
        }

        private async ValueTask InitializeAsync()
        {
            _pager._storeInfo ??= await TEntity.GetStoreInfoAsync(_pager._client, _pager._path, _token);
            _pager._broker ??= await TEntity.GetBrokerAsync(_pager._client, _token);

            _storeCount = _pager._storeInfo.EnabledStoresForHierarchy.Count + 1;

            for (var i = 0; i != _storeCount; i++)
            {
                var store = i == 0
                    ? _pager._storeInfo.Store
                    : await _pager._storeInfo.GetOrComputeStoreAsync(_pager._storeInfo.EnabledStoresForHierarchy[i - 1],
                        _token);

                _enumerators.Insert(
                    i,
                    _isQuery
                        ? store
                            .QueryAsync(_pager._lowerBounds.Value, _pager._direction!.Value, _pager._total!.Value,
                                _token)
                            .GetAsyncEnumerator(_token)
                        : store
                            .GetAllAsync(_token)
                            .GetAsyncEnumerator(_token)
                );
            }

            _hasInit = true;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_enumerationEnded)
                return false;

            if (!_hasInit)
                await InitializeAsync();

            if (_isQuery)
                return await MoveNextQuery();

            return await MoveNextFlat();
        }

        private TModel? GetNextItemInView()
        {
            if (
                _pager is not {_idComparer: not null}
                ||
                _view is null
            ) return null;

            var target = _view.ElementAtOrDefault(_oldestViewIndex);

            for (var i = 0; i != _view.Count; i++)
            {
                var item = _view[i];

                if (target is null || _lastId is null)
                    goto trackItemInView;

                // compare here returns < 0 if 'item' comes before '_lastId'
                var distance = _pager._idComparer.Compare(item.Id, _lastId);

                // if we're checking that 'item' comes after '_lastId', we can negate distance to achieve that.
                if (_pager._direction is Direction.After)
                    distance = -distance;

                if (distance >= 0) continue;

                trackItemInView:

                // verify we're still in bounds
                if (
                    _pager._upperBounds.IsSpecified &&
                    _pager._idComparer.Compare(item.Id, _pager._upperBounds.Value) > 0
                )
                {
                    // don't break since '_view' can be out of order
                    continue;
                }

                target = item;
                _lastId = item.Id;
                _oldestViewIndex = i;
            }

            return target;
        }

        private async ValueTask<bool> MoveNextQuery()
        {
            if (_enumerators is null)
                return false;

            var startIndex = 0;

            if (_view is null)
            {
                _view = new List<TModel>(_storeCount);

                for (var i = 0; i != _storeCount; i++)
                {
                    get_store:

                    if (i >= _enumerators.Count)
                        break;

                    if (!await _enumerators[i].MoveNextAsync())
                    {
                        _storeCount--;
                        await _enumerators[i].DisposeAsync();
                        _enumerators.RemoveAt(i);
                        goto get_store;
                    }

                    _view.Insert(i, _enumerators[i].Current);
                }

                if (_storeCount < _view.Count)
                    _view.RemoveRange(_storeCount, _view.Count - _storeCount);

                var initial = GetNextItemInView();

                if (initial is not null)
                    _page[++startIndex] = initial;
            }

            for (var i = startIndex; i != _pager._pageSize; i++)
            {
                getStore:
                if (_storeCount == 0)
                {
                    _enumerationEnded = true;

                    if (i <= 0) return false;

                    _page = _page[..i];
                    await ProcessPageAsync();
                    return true;
                }

                var store = _enumerators[_oldestViewIndex];

                if (!await store.MoveNextAsync())
                {
                    await _enumerators[_oldestViewIndex].DisposeAsync();
                    _enumerators.RemoveAt(_oldestViewIndex);

                    _view.RemoveAt(_oldestViewIndex);

                    _storeCount--;

                    if (_view.Count > 0)
                    {
                        var element = GetNextItemInView();
                        if (element is not null)
                        {
                            _page[i] = element;
                            continue;
                        }
                    }

                    goto getStore;
                }

                _view.Insert(_oldestViewIndex, store.Current);

                var next = GetNextItemInView();

                if (next is not null)
                {
                    _page[i] = next;
                    continue;
                }

                _enumerationEnded = true;

                if (i <= 0) return false;

                _page = _page[..i];
                break;
            }

            await ProcessPageAsync();
            return true;
        }

        private async ValueTask<bool> MoveNextFlat()
        {
            if (_enumerators is null)
                return false;

            for (var pageIndex = 0; pageIndex != _pager._pageSize; pageIndex++)
            {
                getStore:
                if (_storeCount == 0)
                {
                    _enumerationEnded = true;

                    if (pageIndex <= 0) return false;

                    _page = _page[..pageIndex];
                    break;
                }

                var storeIndex = _storeCount % pageIndex;

                if (!await _enumerators[storeIndex].MoveNextAsync())
                {
                    await _enumerators[storeIndex].DisposeAsync();
                    _enumerators.RemoveAt(storeIndex);

                    _storeCount--;
                    goto getStore;
                }

                _page[pageIndex] = _enumerators[storeIndex].Current;
            }

            await ProcessPageAsync();
            return true;
        }

        private async ValueTask ProcessPageAsync()
        {
            if (_page.Length == 0 || _pager._broker is null)
                return;

            Current = (await _pager._broker.BatchCreateOrUpdateImplicitAsync(_pager._path, _page, token: _token))
                .ToImmutableList();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var enumerator in _enumerators)
            {
                await enumerator.DisposeAsync();
            }

            _enumerators.Clear();
            _enumerators = null!;

            _view?.Clear();

            _page = null!;
            _view = null!;
        }
    }
}
