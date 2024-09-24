using Discord.Models;

namespace Discord.Rest;

public abstract class RestEntity<TId, TModel>(DiscordRestClient client, TId id) :
    RestEntity<TId>(client, id)
    where TId : IEquatable<TId>
    where TModel : IModel
{
    private struct DependentComputed(object? value, int version)
    {
        public object? Value = value;
        public int Version = version;
    }
    private readonly Dictionary<string, object?> _computedCache = new();
    private readonly Dictionary<string, DependentComputed> _dependentComputeCache = new();
    
    private int _computedVersion;
    
    internal abstract TModel Model { get; }

    protected T Computed<T, U>(string name, Func<TModel, U> dependecy, Func<TModel, T> compute)
    {
        var version = HashCode.Combine(name, dependecy(Model));
        
        lock (_dependentComputeCache)
        {
            if (_dependentComputeCache.TryGetValue(name, out var existing))
            {
                if (existing.Version == version && existing.Value is T value)
                    return value;

                existing.Value = value = compute(Model);
                existing.Version = version;
                return value;
            }
            
            var result = compute(Model);
            _dependentComputeCache[name] = new(result, version);
            return result;
        }
    }
    
    protected T Computed<T>(string name, Func<TModel, T> compute)
    {
        PrepareComputedCache();

        lock (_computedCache)
        {
            if (_computedCache.TryGetValue(name, out var cached) && cached is T result)
                return result;
            
            result = compute(Model);
            _computedCache[name] = result;
            return result;
        }
    }

    private void PrepareComputedCache()
    {
        var version = EqualityComparer<TModel>.Default.GetHashCode(Model);

        var result = Interlocked.Exchange(ref _computedVersion, version);

        if (result == version) return;
        
        lock(_computedCache) _computedCache.Clear();
    }
}

public abstract class RestEntity<TId>(DiscordRestClient client, TId id) : 
    IRestEntity<TId>
    where TId : IEquatable<TId>
{
    public DiscordRestClient Client { get; } = client;
    public TId Id { get; } = id;
}

public interface IRestEntity<out TId> : 
    IEntity<TId>,
    IRestClientProvider
    where TId : IEquatable<TId>;