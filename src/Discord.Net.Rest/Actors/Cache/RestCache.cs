using System.Runtime.CompilerServices;

namespace Discord.Rest;

internal sealed class RestCache
{
    internal DiscordRestClient Client { get; }

    private readonly Dictionary<Type, CacheRule> _rules;
    private readonly ConditionalWeakTable<IRestActor, IRestEntity> _localTable;
    private readonly WeakTable<IGlobalEntityCacheKey, IRestEntity> _globalTable;

    private readonly CacheMode _defaultMode;

    public RestCache(DiscordRestClient client)
    {
        Client = client;
        _localTable = new();
        _globalTable = new();
    }

    public TEntity? GetCachedEntity<TId, TEntity>(IRestActor<TId> actor)
        where TId : IEquatable<TId>
    {
        var rule = GetCacheRule(actor.GetType());

        if (rule.CacheEntitiesGlobally)
        {
            var key = new GlobalEntityCacheKey<TId>(actor.GetType(), actor.Id);
            if (_globalTable.TryGetValue(key, out var entity) && entity is TEntity target) return target;
        }

        if (
            rule.CacheEntitiesLocally &&
            _localTable.TryGetValue(actor, out var localEntity) && localEntity is TEntity localTarget
        ) return localTarget;

        return default;
    }

    internal IKeyedCache<TId, TActor> CreateActorsCache<TId, TActor>(
        Type type,
        Func<DiscordRestClient, TId, TActor> factory
    )
        where TId : notnull
        where TActor : class
    {
        var rule = GetCacheRule(type);

        if (rule.CacheActors) return new WeakKeyedCache<TId, TActor>(this, factory);

        return new FactoryCache<TId, TActor>(this, factory);
    }

    internal void CacheEntity<TId, TEntity>(TEntity entity) where TEntity : IRestEntity<TId>
        where TId : IEquatable<TId>
    {
        var actorType = entity.Actor.GetType();
        var rule = GetCacheRule(actorType);

        if (rule.CacheEntitiesGlobally)
        {
            var key = new GlobalEntityCacheKey<TId>(actorType, entity.Id);
            _globalTable[key] = entity;
        }
        else if (rule.CacheEntitiesLocally)
        {
            _localTable.AddOrUpdate(entity.Actor, entity);
        }
    }

    private CacheRule GetCacheRule(Type type)
    {
        if (!_rules.TryGetValue(type, out var rule))
            _rules[type] = rule = new(type, _defaultMode);

        return rule;
    }

    private interface IGlobalEntityCacheKey;

    private sealed record GlobalEntityCacheKey<TId>(Type ActorType, TId Id) : IGlobalEntityCacheKey;
}

internal interface IKeyedCache<in TKey, out TValue>
    where TKey : notnull
    where TValue : class
{
    TValue Get(TKey key);
}

internal sealed class FactoryCache<TKey, TValue>(
    RestCache cache,
    Func<DiscordRestClient, TKey, TValue> factory
) : IKeyedCache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    public TValue Get(TKey key) => factory(cache.Client, key);
}

internal sealed class WeakKeyedCache<TKey, TValue> : IKeyedCache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private readonly RestCache _cache;
    private readonly Func<DiscordRestClient, TKey, TValue> _factory;
    private readonly WeakTable<TKey, TValue> _table;

    public WeakKeyedCache(RestCache cache, Func<DiscordRestClient, TKey, TValue> factory)
    {
        _cache = cache;
        _factory = factory;
        _table = new();
    }

    public TValue Get(TKey key) => _table.GetOrAdd(key, _factory, _cache.Client);
}

[Flags]
public enum CacheMode
{
    None = 0,
    CacheActors = 1 << 0,
    CacheEntitiesLocally = 1 << 2,
    CacheEntitiesGlobally = 1 << 3
}

public readonly record struct CacheRule(
    Type Target,
    CacheMode Mode
)
{
    public bool CacheActors => (Mode & CacheMode.CacheActors) != 0;
    public bool CacheEntitiesLocally => (Mode & CacheMode.CacheEntitiesLocally) != 0;
    public bool CacheEntitiesGlobally => (Mode & CacheMode.CacheEntitiesGlobally) != 0;
}