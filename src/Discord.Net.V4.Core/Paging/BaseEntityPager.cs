using Discord.Paging;
using System.Collections.Immutable;

namespace Discord;

public class EntityPager<TEntity, TModel>(
    IDiscordClient client,
    int? pageSize,
    int? total,
    IApiOutRoute<TModel> route,
    Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> factory,
    Func<EntityPager<TEntity, TModel>, TModel, IApiOutRoute<TModel>?> nextPage,
    RequestOptions? options
):
    IAsyncPaged<TEntity>
    where TModel : class
{
    public int? PageSize { get; } = pageSize;
    public int? Total { get; } = total;

    protected IDiscordClient Client { get; } = client;

    private readonly Func<EntityPager<TEntity, TModel>, TModel, IApiOutRoute<TModel>?> _nextPage = nextPage;
    private readonly IApiOutRoute<TModel> _startRoute = route;
    private readonly RequestOptions _options = options ?? client.DefaultRequestOptions;
    private readonly Func<EntityPager<TEntity, TModel>, TModel, IEnumerable<TEntity>> _factory = factory;

    protected sealed class Enumerator(EntityPager<TEntity, TModel> pager, CancellationToken token)
        : IAsyncEnumerator<IReadOnlyCollection<TEntity>>
    {
        public IReadOnlyCollection<TEntity> Current { get; private set; } = [];
        public int Count { get; private set; }

        private IApiOutRoute<TModel>? _nextRoute = pager._startRoute;

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_nextRoute is null)
                return false;

            if (Count >= pager.Total)
                return false;

            if (pager.PageSize.HasValue && Current.Count < pager.PageSize.Value)
                return false;

            var model = await pager.Client.RestApiClient.ExecuteAsync(_nextRoute, pager._options, token);

            if (model is null)
                return false;

            Current = pager._factory(pager, model).ToImmutableArray();
            Count += Current.Count;

            _nextRoute = pager._nextPage(pager, model);

            return true;
        }


        public ValueTask DisposeAsync()
        {
            Current = null!;
            _nextRoute = null!;
            return default;
        }
    }

    public IAsyncEnumerator<IReadOnlyCollection<TEntity>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return new Enumerator(this, cancellationToken);
    }
}
