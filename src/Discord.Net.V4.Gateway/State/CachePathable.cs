using Discord.Models;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway.State;

internal sealed class CachePathable : IPathable, IReadOnlyDictionary<Type, object>
{
    public static readonly IPathable Default = new CachePathable();

    //private readonly LinkedList<IIdentifiable<TId, TEntity, TModel>>
    private readonly SortedDictionary<Type, object> _path = [];

    public void Push<TId, TEntity, TModel>(IIdentifiable<TId, TEntity, TModel> identity)
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (_path.ContainsKey(typeof(TEntity)))
            throw new ArgumentException($"path already contains a definition for {typeof(TEntity)}");

        _path[typeof(TEntity)] = identity.Id;
    }

    public TId Require<TId, TEntity>() where TId : IEquatable<TId> where TEntity : class, IEntity<TId>
    {
        if (_path.TryGetValue(typeof(TEntity), out var value) && value is TId id)
            return id;

        throw new KeyNotFoundException($"No entry exists for {typeof(TEntity)}");
    }

    public bool TryGet<TId, TEntity>(out TId? id) where TId : IEquatable<TId> where TEntity : class, IEntity<TId>
    {
        id = default;

        if (_path.TryGetValue(typeof(TEntity), out var value) && value is TId idValue)
            id = idValue;

        return id is not null;
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _path.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_path).GetEnumerator();

    public int Count => _path.Count;

    public bool ContainsKey(Type key) => _path.ContainsKey(key);

    public bool TryGetValue(Type key, [MaybeNullWhen(false)] out object value) => _path.TryGetValue(key, out value);

    public object this[Type key] => _path[key];

    public IEnumerable<Type> Keys => _path.Keys;

    public IEnumerable<object> Values =>  _path.Values;
}
