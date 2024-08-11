using Discord.Models;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

public sealed partial class CachePathable : IPathable, IReadOnlyDictionary<Type, object>, IDisposable
{
    public static readonly CachePathable Empty = new CachePathable().MakeReadOnly();

    //private readonly LinkedList<IIdentifiable<TId, TEntity, TModel>>
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

    private bool TryGetIdentity<TId, TEntity>([MaybeNullWhen(false)]out IIdentifiable<TId> identity)
        where TId : IEquatable<TId>
    {
        if (_identities.TryGetValue(typeof(TEntity), out var value))
            return (identity = value as IIdentifiable<TId>) is not null;

        // slow search
        foreach (var (type, raw) in _identities)
        {
            if(typeof(TEntity).IsAssignableTo(type))
                return (identity = raw as IIdentifiable<TId>) is not null;
        }

        identity = null;
        return false;
    }

    [return: NotNullIfNotNull(nameof(fallback))]
    internal TIdentity? GetIdentity<[TransitiveFill] TIdentity, TId, TEntity, TModel>(Template<TIdentity> template, TId? fallback = default)
        where TIdentity : class, IIdentifiable<TId, TEntity, TModel>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetIdentity<TId, TEntity>(out var value) && value is TIdentity identity)
            return identity;

        if(fallback is not null)
            return (TIdentity)IIdentifiable<TId, TEntity, TModel>.Of(fallback);

        return null;
    }

    internal TIdentity RequireIdentity<[TransitiveFill] TIdentity, TId, TEntity, TModel>(Template<TIdentity> template)
        where TIdentity : IIdentifiable<TId, TEntity, TModel>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (TryGetIdentity<TId, TEntity>(out var value) && value is TIdentity identity)
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

    public bool TryGet<TId, TEntity>([MaybeNullWhen(false)] out TId id) where TId : IEquatable<TId> where TEntity : class, IEntity<TId>
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

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_identities).GetEnumerator();

    public int Count => _identities.Count;

    public bool ContainsKey(Type key) => _identities.ContainsKey(key);

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out object value) => _identities.TryGetValue(key, out value);

    public object this[Type key] => _identities[key];

    public IEnumerable<Type> Keys => _identities.Keys;

    public IEnumerable<object> Values =>  _identities.Values;

    public void Dispose()
    {
        if(!_pathables.IsReadOnly)
            _pathables.Clear();

        if(!_identities.IsReadOnly)
            _identities.Clear();

        _pathables = null!;
        _identities = null!;
    }
}
