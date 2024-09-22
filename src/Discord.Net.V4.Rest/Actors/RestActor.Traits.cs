namespace Discord.Rest;

public abstract partial class RestActor<TSelf, TId, TEntity, TModel> :
    IRestTraitProvider
{
    private readonly Dictionary<string, object> _traits = new();
    
    TTrait IRestTraitProvider.GetOrCreateTraitData<TTrait, TSelf1>(
        TSelf1 arg,
        string name,
        Func<TSelf1, TTrait> factory)
    {
        if (arg is not TSelf)
            throw new InvalidCastException($"{GetType()} is not of type {typeof(TSelf)}");

        lock (Identity)
        {
            if (_traits.TryGetValue(name, out var traitData))
                return (TTrait) traitData;

            var trait = factory(arg);
            _traits[name] = trait;
            return trait;
        }
    }

    void IRestTraitProvider.ClearTraitData(string name)
    {
        lock (Identity)
        {
            if (!_traits.Remove(name, out var trait))
                return;
            
            if(trait is IDisposable disposable)
                disposable.Dispose();
        }
    }
}

public interface IRestTraitProvider<out TSelf>
    where TSelf : IRestTraitProvider<TSelf>, IRestTraitProvider
{
    internal T GetOrCreateTraitData<T>(string name, Func<TSelf, T> factory)
        where T : notnull
    {
        if (this is not TSelf self)
            throw new InvalidCastException($"{GetType()} is not of type {typeof(TSelf)}");

        return self.GetOrCreateTraitData(self, name, factory);
    }

    internal void ClearTraitData(string name)
    {
        if (this is not IRestTraitProvider self)
            throw new InvalidCastException($"{GetType()} is not of type {typeof(TSelf)}");
        
        self.ClearTraitData(name);
    }
}

public interface IRestTraitProvider
{
    internal TTrait GetOrCreateTraitData<TTrait, TSelf>(TSelf self, string name, Func<TSelf, TTrait> factory)
        where TTrait : notnull;

    internal void ClearTraitData(string name);
}