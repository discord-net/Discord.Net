using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Paging;

public sealed class BasicEntityAsyncPaged<TId, TEntity, TModel>(
    IEntityProvider<TEntity, TModel> entityProvider,
    int pageSize,
    BasicEntityAsyncPaged<TId, TEntity, TModel>.GetNextRoute routeProvider,
    BasicEntityAsyncPaged<TId, TEntity, TModel>.GetNextPageKey nextKey,
    RequestOptions? options = null)
    : IAsyncPaged<TEntity>
    where TEntity : IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel
{
    public delegate ApiRoute<TModel[]> GetNextRoute(TId? current);

    public delegate TId? GetNextPageKey(TModel[] models);

    public int PageSize { get; } = pageSize;

    private readonly IEntityProvider<TEntity, TModel> _entityProvider = entityProvider;
    private readonly GetNextRoute _routeProvider = routeProvider;
    private readonly GetNextPageKey _nextKey = nextKey;

    public IAsyncEnumerator<IReadOnlyCollection<TEntity>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
        => new Enumerator(this, options ?? _entityProvider.Client.DefaultRequestOptions, cancellationToken);

    private sealed class Enumerator(
        BasicEntityAsyncPaged<TId, TEntity, TModel> pager,
        RequestOptions options,
        CancellationToken token
    ) : IAsyncEnumerator<IReadOnlyCollection<TEntity>>
    {
        public IReadOnlyCollection<TEntity> Current { get; private set; } = [];

        private TId? _currentId = default;

        private readonly CancellationToken _token = token;
        private readonly RequestOptions _options = options;

        private readonly BasicEntityAsyncPaged<TId, TEntity, TModel> _pager = pager;


        public async ValueTask<bool> MoveNextAsync()
        {
            var route = _pager._routeProvider(_currentId);

            var result = await _pager._entityProvider.Client.RestApiClient.ExecuteAsync(route, _options, _token);

            if (result is null || result.Length == 0)
                return false;

            _currentId = _pager._nextKey(result);

            Current = result.Select(x => _pager._entityProvider.CreateEntity(x)).ToImmutableArray();
            return true;
        }


        public ValueTask DisposeAsync()
        {
            Current = null!;
            return ValueTask.CompletedTask;
        }
    }
}
