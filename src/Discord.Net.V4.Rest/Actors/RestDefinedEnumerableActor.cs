using System.Collections;
using System.Collections.Immutable;

namespace Discord.Rest.Actors;

public sealed partial class RestManagedEnumerableActor<TActor, TId, TEntity, TCoreEntity> :
    RestDefinedEnumerableActor<TActor, TId, TEntity, TCoreEntity>,
    IEnumerable<TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, TCoreEntity, IProxied<TActor>
    where TCoreEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    public override IReadOnlyCollection<TId> Ids => _entities.Keys;

    private readonly Dictionary<TId, TEntity> _entities;
    private readonly Func<TId, TActor> _actorFactory;
    private readonly DiscordRestClient _client;
    private readonly IApiOutRoute<IEnumerable<TEntity>> _apiRoute;

    public RestManagedEnumerableActor(
        DiscordRestClient client,
        IEnumerable<TEntity> entities,
        Func<TId, TActor> actorFactory,
        IApiOutRoute<IEnumerable<TEntity>> apiRoute)
    {
        _apiRoute = apiRoute;
        _client = client;
        _actorFactory = actorFactory;
        _entities = entities.ToDictionary(x => x.Id, x => x);
    }

    public override TActor Specifically(TId id)
        => _entities.TryGetValue(id, out var entity) ? entity.ProxiedValue : _actorFactory(id);

    public override async Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null,
        CancellationToken token = default)
    {
        if (options?.ForceUpdate != true) return _entities.Values;

        var updated =
            await _client.ApiClient.ExecuteAsync(_apiRoute, options, token);

        if (updated is null)
        {
            _entities.Clear();
            return [];
        }

        Update(updated);

        return _entities.Values;
    }

    internal void Update(IEnumerable<TEntity> entities)
    {
        lock (this)
        {
            _entities.Clear();
            foreach (var entity in entities)
                _entities[entity.Id] = entity;
        }
    }

    public override IEnumerable<TActor> Specifically(IEnumerable<TId> ids)
        => ids.Select(Specifically);

    public IEnumerator<TEntity> GetEnumerator() => _entities.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract partial class RestDefinedEnumerableActor<TActor, TId, TEntity, TCoreEntity> :
    IDefinedEnumerableActor<TActor, TId, TCoreEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : class, IEntity<TId>, TCoreEntity
    where TCoreEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    public abstract IReadOnlyCollection<TId> Ids { get; }

    public abstract TActor Specifically(TId id);

    public abstract Task<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null,
        CancellationToken token = default);

    async Task<IReadOnlyCollection<TCoreEntity>> IEnumerableActor<TId, TCoreEntity>.AllAsync(RequestOptions? options,
        CancellationToken token)
        => await AllAsync(options, token);

    public abstract IEnumerable<TActor> Specifically(IEnumerable<TId> ids);
}
