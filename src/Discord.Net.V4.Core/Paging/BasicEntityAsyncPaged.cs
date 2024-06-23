using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Paging;

public sealed class BasicEntityAsyncPaged<TId, TEntity, TModel>(
    IDiscordClient client,
    Func<TModel, TEntity> entityFactory,
    int pageSize,
    BasicEntityAsyncPaged<TId, TEntity, TModel>.GetNextRoute routeProvider,
    BasicEntityAsyncPaged<TId, TEntity, TModel>.GetNextPageKey nextKey,
    RequestOptions? options = null)
    : IAsyncPaged<TEntity>
    where TEntity : IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel
{
    public delegate TId? GetNextPageKey(TModel[] models);

    public delegate IApiOutRoute<TModel[]> GetNextRoute(TId? current);

    private IDiscordClient Client { get; } = client;
    private readonly Func<TModel, TEntity> _entityFactory = entityFactory;
    private readonly GetNextPageKey _nextKey = nextKey;
    private readonly GetNextRoute _routeProvider = routeProvider;

    public int? PageSize { get; } = pageSize;

    public IAsyncEnumerator<IReadOnlyCollection<TEntity>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
        => new Enumerator(this, options ?? Client.DefaultRequestOptions, cancellationToken);

    private sealed class Enumerator(
        BasicEntityAsyncPaged<TId, TEntity, TModel> pager,
        RequestOptions options,
        CancellationToken token
    ) : IAsyncEnumerator<IReadOnlyCollection<TEntity>>
    {
        private TId? _currentId;
        public IReadOnlyCollection<TEntity> Current { get; private set; } = [];


        public async ValueTask<bool> MoveNextAsync()
        {
            var route = pager._routeProvider(_currentId);

            var result = await pager.Client.RestApiClient.ExecuteAsync(route, options, token);

            if (result is null || result.Length == 0)
                return false;

            _currentId = pager._nextKey(result);

            Current = result.Select(x => pager._entityFactory(x)).ToImmutableArray();
            return true;
        }


        public ValueTask DisposeAsync()
        {
            Current = null!;
            return ValueTask.CompletedTask;
        }
    }
}
