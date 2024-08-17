using Discord.Models;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway.State;

public sealed class CachePathable : IPathable, IReadOnlyDictionary<Type, object>, IDisposable
{
    public static readonly CachePathable Empty = new CachePathable().MakeReadOnly();

    public bool IsReadOnly { get; private set; }
    
    private IDictionary<Type, object> _identities = new Dictionary<Type, object>();

    private IList<IPathable> _pathables = new List<IPathable>();

    private HashSet<IModel> _enrichedModels = new();
    
    private CachePathable MakeReadOnly()
    {
        if (!_identities.IsReadOnly)
            _identities = _identities.ToImmutableDictionary();

        if (!_pathables.IsReadOnly)
            _pathables = _pathables.ToImmutableList();

        IsReadOnly = true;
        
        return this;
    }

    private void EnrichFrom(IModel model)
    {
        _enrichedModels.Add(model);

        if (model is IModelSource source)
            _enrichedModels.UnionWith(source.GetDefinedModels());
    }

    private bool TryGetFromModels<TId, TEntity>([MaybeNullWhen(false)] out TId id)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>
    {
        if (_enrichedModels.Count == 0)
        {
            id = default;
            return false;
        }

        // we don't care if the entity implements multiple 'IEntityOf' interfaces since they all will use the 
        // same underlying id.
        var entityModel = typeof(TEntity).GetInterface("IEntityOf`1")?.GenericTypeArguments[0];
        
        if (entityModel is null)
        {
            id = default;
            return false;
        }
        
        foreach (var model in _enrichedModels)
        {
            if (model is ILinkingModel<TId> link && link.TryGetId(entityModel, out id))
            {
                return true;
            }
        }

        id = default;
        return false;
    }

    public bool Contains<TId, TEntity>(TId id)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>
        => TryGet<TId, TEntity>(out var existing) && id.Equals(existing);

    private bool TryGetIdentity<TId, TEntity>([MaybeNullWhen(false)] out IIdentifiable<TId> identity)
        where TId : IEquatable<TId>
    {
        if (_identities.TryGetValue(typeof(TEntity), out var value))
            return (identity = value as IIdentifiable<TId>) is not null;

        // slow search
        foreach (var (type, raw) in _identities)
        {
            if (type.IsAssignableTo(typeof(TEntity)))
                return (identity = raw as IIdentifiable<TId>) is not null;
        }

        identity = null;
        return false;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    internal IIdentifiable<TId, TEntity, TActor, TModel>? GetIdentity<TId, TEntity, TActor, TModel>(
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
        TId? fallback
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        return
            GetIdentity(Template.Of<IIdentifiable<TId, TEntity, TModel>>())
                as IIdentifiable<TId, TEntity, TActor, TModel>
            ?? (
                fallback is not null
                    ? IIdentifiable<TId, TEntity, TActor, TModel>.Of(fallback)
                    : null
            );
    }

    internal IIdentifiable<TId, TEntity, TActor, TModel>? GetIdentity<TId, TEntity, TActor, TModel>(
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        return GetIdentity(Template.Of<IIdentifiable<TId, TEntity, TModel>>())
            as IIdentifiable<TId, TEntity, TActor, TModel>;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    internal IIdentifiable<TId, TEntity, TModel>? GetIdentity<TId, TEntity, TModel>(
        Template<IIdentifiable<TId, TEntity, TModel>> template,
        TId? fallback
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        return GetIdentity(template) ?? (
            fallback is not null    
                ? IIdentifiable<TId, TEntity, TModel>.Of(fallback)
                : null
        );
    }
    
    internal IIdentifiable<TId, TEntity, TModel>? GetIdentity<TId, TEntity, TModel>(
        Template<IIdentifiable<TId, TEntity, TModel>> template
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetIdentity<TId, TEntity>(out var value) && value is IIdentifiable<TId, TEntity, TModel> identity)
            return identity;

        return null;
    }

    internal IIdentifiable<TId, TEntity, TModel> RequireIdentity<TId, TEntity, TModel>(
        Template<IIdentifiable<TId, TEntity, TModel>> template
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetIdentity<TId, TEntity>(out var value) && value is IIdentifiable<TId, TEntity, TModel> identity)
            return identity;

        throw new KeyNotFoundException($"Couldn't find an identity for '{typeof(TEntity)}'");
    }

    internal IIdentifiable<TId, TEntity, TActor, TModel> RequireIdentity<TId, TEntity, TActor, TModel>(
        Template<IIdentifiable<TId, TEntity, TActor, TModel>> template
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (TryGetIdentity<TId, TEntity>(out var value) &&
            value is IIdentifiable<TId, TEntity, TActor, TModel> identity)
            return identity;

        throw new KeyNotFoundException($"Couldn't find an identity for '{typeof(TEntity)}'");
    }

    public void Add(IPathable pathable)
        => _pathables.Add(pathable);

    public void Add(IModel model)
    {
        if (!IsReadOnly) EnrichFrom(model);
    }

    public void Add<TId, TEntity, TModel>(IIdentifiable<TId, TEntity, TModel> identity)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (_identities.ContainsKey(typeof(TEntity)))
            throw new ArgumentException($"path already contains a definition for {typeof(TEntity)}");

        _identities[typeof(TEntity)] = identity;
    }

    public TId Require<TId, TEntity>() where TId : IEquatable<TId> where TEntity : class, IEntity<TId>
    {
        if (TryGetIdentity<TId, TEntity>(out var identity))
            return identity.Id;

        if (TryGetFromModels<TId, TEntity>(out var id))
            return id;
        
        foreach (var pathable in _pathables)
        {
            if (pathable.TryGet<TId, TEntity>(out id))
                return id;
        }

        throw new KeyNotFoundException($"No entry exists for {typeof(TEntity)}");
    }

    public bool TryGet<TId, TEntity>([MaybeNullWhen(false)] out TId id)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>
    {
        id = default;

        if (TryGetIdentity<TId, TEntity>(out var identity))
        {
            id = identity.Id;
            return true;
        }

        if (TryGetFromModels<TId, TEntity>(out id))
            return true;

        foreach (var pathable in _pathables)
        {
            if (pathable.TryGet<TId, TEntity>(out id))
                return true;
        }

        return id is not null;
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _identities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _identities).GetEnumerator();

    public int Count => _identities.Count;

    public bool ContainsKey(Type key) => _identities.ContainsKey(key);

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out object value) =>
        _identities.TryGetValue(key, out value);

    public object this[Type key] => _identities[key];

    public IEnumerable<Type> Keys => _identities.Keys;

    public IEnumerable<object> Values => _identities.Values;

    public void Dispose()
    {
        if (!_pathables.IsReadOnly)
            _pathables.Clear();

        if (!_identities.IsReadOnly)
            _identities.Clear();

        _enrichedModels.Clear();

        _enrichedModels = null!;
        _pathables = null!;
        _identities = null!;
    }
}