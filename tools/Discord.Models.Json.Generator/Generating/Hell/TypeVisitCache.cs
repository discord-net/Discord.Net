using System.Diagnostics.CodeAnalysis;

namespace Discord.Models.Json.Generator.Hell;

public sealed class TypeVisitCache
{
    public readonly HashSet<Type> Visited = [];
    private readonly Queue<Type> _queue;

    public TypeVisitCache(params IEnumerable<Type> types)
    {
        _queue = new Queue<Type>(types);
    }
    
    public bool Add(Type type)
    {
        if (Visited.Contains(type)) return false;
        _queue.Enqueue(type);
        return true;
    }

    public bool TryGetNext([MaybeNullWhen(false)] out Type type)
    {
        while (_queue.TryDequeue(out type))
        {
            if(!Visited.Add(type)) continue;
            return true;
        }

        return false;
    }
}