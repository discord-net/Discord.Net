using Discord;
using Discord.Models;
using Discord.Rest;
using Discord.Rest.Actors;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

public static class RestManagedEnumerableActor
{
    public static RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel> Create<TActor, TId, TEntity, TCore,
        TModel>(
        DiscordRestClient client,
        IEnumerable<TModel> models,
        Func<TId, TActor> actorFactory,
        IApiOutRoute<IEnumerable<TModel>> route)
        where TActor : IActor<TId, TEntity>
        where TEntity :
            RestEntity<TId>,
            TCore,
            IProxied<TActor>,
            IEntityOf<TModel>,
            IConstructable<TEntity, TModel, DiscordRestClient>
        where TId : IEquatable<TId>
        where TCore : class, IEntity<TId>
        where TModel : class, IEntityModel<TId>
    {
        return new RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel>(
            client,
            models,
            actorFactory,
            model => TEntity.Construct(client, model),
            route
        );
    }

    public static RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel> Create<TActor, TId, TEntity, TCore,
        TModel, TContext>(
        DiscordRestClient client,
        IEnumerable<TModel> models,
        Func<TId, TActor> actorFactory,
        IApiOutRoute<IEnumerable<TModel>> route,
        TContext context)
        where TActor : IActor<TId, TEntity>
        where TEntity :
        RestEntity<TId>,
        TCore,
        IProxied<TActor>,
        IEntityOf<TModel>,
        IContextConstructable<TEntity, TModel, TContext, DiscordRestClient>
        where TId : IEquatable<TId>
        where TCore : class, IEntity<TId>
        where TModel : class, IEntityModel<TId>
    {
        return new RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel>(
            client,
            models,
            actorFactory,
            model => TEntity.Construct(client, model, context),
            route
        );
    }
}

public sealed class RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel> :
    IDefinedEnumerableActor<TActor, TId, TCore>,
    IReadOnlyDictionary<TId, TEntity>
    where TActor : IActor<TId, TEntity>
    where TEntity : RestEntity<TId>, TCore, IProxied<TActor>, IEntityOf<TModel>
    where TId : IEquatable<TId>
    where TCore : class, IEntity<TId>
    where TModel : class, IEntityModel<TId>
{
    public IReadOnlyDictionary<TId, TEntity> All { get; private set; }

    public IReadOnlyCollection<TId> Ids
        => _keys ??= All.Keys.ToImmutableArray();

    public IReadOnlyCollection<TEntity> Values
        => _values ??= All.Values.ToImmutableArray();

    public TEntity this[TId key] => All[key];

    public int Count => All.Count;

    private IEnumerable<TModel> _models;
    private IReadOnlyCollection<TId>? _keys;
    private IReadOnlyCollection<TEntity>? _values;

    private readonly DiscordRestClient _client;
    private readonly RestIndexableActor<TActor, TId, TEntity> _indexableActor;
    private readonly Func<TModel, TEntity> _entityFactory;
    private readonly IApiOutRoute<IEnumerable<TModel>> _route;

    public RestManagedEnumerableActor(
        DiscordRestClient client,
        IEnumerable<TModel> models,
        Func<TId, TActor> actorFactory,
        Func<TModel, TEntity> entityFactory,
        IApiOutRoute<IEnumerable<TModel>> route)
    {
        var x =
            _client = client;
        _models = models;
        _indexableActor = new(actorFactory);
        _entityFactory = entityFactory;
        _route = route;

        All = _models.ToImmutableDictionary(x => x.Id, _entityFactory);
    }

    internal void Update(IEnumerable<TModel> models)
    {
        lock (this)
        {
            var entityModels = models as TModel[] ?? models.ToArray();

            if (entityModels.SequenceEqual(_models))
                return;

            All = (_models = entityModels).ToImmutableDictionary(x => x.Id, _entityFactory);
            _keys = null;
            _values = null;
        }
    }

    public bool Contains(TId id) => All.ContainsKey(id);

    async ValueTask<IReadOnlyCollection<TCore>> IEnumerableActor<TId, TCore>.AllAsync(RequestOptions? options,
        CancellationToken token)
    {
        if (options?.AllowCached ?? false)
            return Values;

        var models = await _client.ApiClient.ExecuteAsync(_route, options ?? _client.DefaultRequestOptions, token);

        if (models is null)
            return [];

        Update(models);
        return Values;
    }

    IEnumerable<TActor> IDefinedEnumerableActor<TActor, TId, TCore>.Specifically(IEnumerable<TId> ids)
        => ids.Select(
            id => All.TryGetValue(id, out var entity) ? entity.ProxiedValue : _indexableActor.Specifically(id));

    TActor IIndexableActor<TActor, TId, TCore>.Specifically(TId id)
        => All.TryGetValue(id, out var entity) ? entity.ProxiedValue : _indexableActor.Specifically(id);

    [MustDisposeResource]
    public IEnumerator<KeyValuePair<TId, TEntity>> GetEnumerator() => All.GetEnumerator();

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)All).GetEnumerator();

    bool IReadOnlyDictionary<TId, TEntity>.ContainsKey(TId key) => Contains(key);

    public bool TryGetValue(TId key, [MaybeNullWhen(false)] out TEntity value) => All.TryGetValue(key, out value);

    IEnumerable<TEntity> IReadOnlyDictionary<TId, TEntity>.Values => Values;

    IEnumerable<TId> IReadOnlyDictionary<TId, TEntity>.Keys => Ids;
}
