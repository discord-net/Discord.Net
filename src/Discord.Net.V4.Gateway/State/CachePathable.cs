using Discord.Models;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

public sealed class CachePathable : IPathable, IReadOnlyDictionary<Type, object>, IDisposable
{
    public static readonly CachePathable Empty = new CachePathable().MakeReadOnly();

    private IDictionary<Type, object> _identities = new Dictionary<Type, object>();

    private IList<IPathable> _pathables = new List<IPathable>();

    private CachePathable MakeReadOnly()
    {
        if (!_identities.IsReadOnly)
            _identities = _identities.ToImmutableDictionary();

        if (!_pathables.IsReadOnly)
            _pathables = _pathables.ToImmutableList();

        return this;
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
            if (typeof(TEntity).IsAssignableTo(type))
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

        foreach (var pathable in _pathables)
        {
            if (pathable.TryGet<TId, TEntity>(out var id))
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
            id = identity.Id;

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

        _pathables = null!;
        _identities = null!;
    }
}