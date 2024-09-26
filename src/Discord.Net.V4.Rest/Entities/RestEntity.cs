using Discord.Models;

namespace Discord.Rest;

public abstract class RestEntity<TId, TModel>(DiscordRestClient client, TId id) :
    RestEntity<TId>(client, id),
    IRestEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IModel
{
    private int _computedVersion;
    
    internal abstract TModel Model { get; }

    ref int IRestEntityComputedProvider.ComputedVersion => ref _computedVersion;

    Dictionary<string, object?> IRestEntityComputedProvider.ComputedCache { get; } = new();

    Dictionary<string, IRestEntityComputedProvider.DependentComputed>
        IRestEntityComputedProvider.DependentComputeCache { get; } = new();

    TModel IRestEntity<TId, TModel>.Model => Model;
}

public abstract class RestEntity<TId>(DiscordRestClient client, TId id) : 
    IRestEntity<TId>
    where TId : IEquatable<TId>
{
    public DiscordRestClient Client { get; } = client;
    public TId Id { get; } = id;
}

public interface IRestEntity<out TId, TModel> : 
    IRestEntityComputedProvider
    where TId : IEquatable<TId>
    where TModel : class, IModel
{
    internal TModel Model { get; }
    
    protected T Computed<T, U>(
        string name,
        Func<TModel, U> dependecy,
        Func<TModel, T> compute,
        TModel? model = null)
    {
        model ??= Model;
        
        var version = HashCode.Combine(name, dependecy(model));
        
        lock (DependentComputeCache)
        {
            if (DependentComputeCache.TryGetValue(name, out var existing))
            {
                if (existing.Version == version && existing.Value is T value)
                    return value;

                existing.Value = value = compute(model);
                existing.Version = version;
                return value;
            }
            
            var result = compute(model);
            DependentComputeCache[name] = new(result, version);
            return result;
        }
    }
    
    protected T Computed<T>(
        string name,
        Func<TModel, T> compute,
        TModel? model = null)
    {
        model ??= Model;
        
        PrepareComputedCache(model);

        lock (ComputedCache)
        {
            if (ComputedCache.TryGetValue(name, out var cached) && cached is T result)
                return result;
            
            result = compute(model);
            ComputedCache[name] = result;
            return result;
        }
    }

    private void PrepareComputedCache(TModel? model = null)
    {
        model ??= Model;
        
        var version = EqualityComparer<TModel>.Default.GetHashCode(model);

        var result = Interlocked.Exchange(ref ComputedVersion, version);

        if (result == version) return;
        
        lock(ComputedCache) ComputedCache.Clear();
    }
}

public interface IRestEntityComputedProvider
{
    protected ref int ComputedVersion { get; }
    protected Dictionary<string, object?> ComputedCache { get; }
    protected Dictionary<string, DependentComputed> DependentComputeCache { get; }
    
    protected struct DependentComputed(object? value, int version)
    {
        public object? Value = value;
        public int Version = version;
    }
}

public interface IRestEntity<out TId> : 
    IEntity<TId>,
    IRestClientProvider
    where TId : IEquatable<TId>;