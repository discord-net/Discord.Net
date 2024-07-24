using Discord;
using Discord.Models;
using Discord.Rest;
using Discord.Rest.Actors;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

internal static partial class RestManagedEnumerableActor
{
    public static RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel> Create<
        [TransitiveFill]TActor,
        TId,
        TEntity,
        [Not(nameof(TEntity)), Interface]TCore,
        TModel
    >(
        Template<TActor> template,
        DiscordRestClient client,
        IEnumerable<TModel> models,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, IIdentifiable<TId, TEntity, TActor, TModel>, TActor> actorFactory,
        [VariableFuncArgs(InsertAt = 1)] Func<DiscordRestClient, TModel, TEntity> entityFactory,
        IApiOutRoute<IEnumerable<TModel>> route
    )
        where TActor :
            class,
            IRestActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>>
        where TEntity :
            RestEntity<TId>,
            TCore,
            IProxied<TActor>,
            IEntityOf<TModel>
        where TId : IEquatable<TId>
        where TCore : class, IEntity<TId>
        where TModel : class, IEntityModel<TId>
    {
        return new RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel>(
            client,
            models,
            id => actorFactory(client, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id)),
            model => entityFactory(client, model),
            route
        );
    }
}

public sealed class RestManagedEnumerableActor<TActor, TId, TEntity, TCore, TModel> :
    RestEnumerableIndexableActor<TActor, TId, TEntity, TCore, IEnumerable<TModel>>,
    IDefinedEnumerableActor<TActor, TId, TCore>,
    IReadOnlyDictionary<TId, TEntity>
    where TActor : class, IRestActor<TId, TEntity>
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

    private ImmutableArray<TId>? _keys;
    private ImmutableArray<TEntity>? _values;

    private readonly RestIndexableActor<TActor, TId, TEntity> _indexableActor;
    private readonly Func<TModel, TEntity> _entityFactory;

    public RestManagedEnumerableActor(
        DiscordRestClient client,
        IEnumerable<TModel> models,
        Func<TId, TActor> actorFactory,
        Func<TModel, TEntity> entityFactory,
        IApiOutRoute<IEnumerable<TModel>> route
    ) : base(
        actorFactory,
        models => models.Select(entityFactory),
        (options, token) => client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token)
    )
    {
        _indexableActor = new(actorFactory);
        _entityFactory = entityFactory;

        All = models.ToImmutableDictionary(x => x.Id, _entityFactory);
    }

    internal void Update(IEnumerable<TEntity> entities)
    {
        var immutableEntities = entities as TEntity[] ?? entities.ToArray();

        lock (this)
        {
            if (Values.SequenceEqual(immutableEntities))
                All = immutableEntities.ToImmutableDictionary(x => x.Id);
        }
    }

    internal void Update(IEnumerable<TModel> models)
    {
        lock (this)
        {
            var entityModels = models as TModel[] ?? models.ToArray();
            All = entityModels.ToImmutableDictionary(x => x.Id, _entityFactory);
        }
    }

    public override TActor Specifically(TId id)
        => All.GetValueOrDefault(id)?.ProxiedValue ?? base.Specifically(id);

    public bool Contains(TId id) => All.ContainsKey(id);

    public override async ValueTask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null,
        CancellationToken token = default)
    {
        if ((options?.AllowCached ?? false) && _values is not null)
            return _values;

        var entities = await base.AllAsync(options, token);

        Update(entities);

        return entities;
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
